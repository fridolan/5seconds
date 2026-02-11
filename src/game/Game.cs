using OpenTK.Windowing.Common;

namespace fiveSeconds
{
    public enum GameState
    {
        INPUT,
        UPDATE,
        PAUSE,
        LOBBY,
        GAMESTART
    }

    public class Game
    {
        public GameState State = GameState.LOBBY;
        public Random Random = new();
        public Stage CurrentStage;

        public bool ManuallyPaused = false;
        public float InputTimeLeft = 0;
        public float InputPhaseLength = 6;

        private bool firstUpdateTick = false;

        public void OnLoad()
        {
            /* CurrentStage = new Cave1()
            {
                Game = this,
            }; */
        }

        public Game()
        {
            OnLoad();
        }

        public void OnUpdateFrame(FrameEventArgs args)
        {
            float dT = (float)args.Time;
            if (State == GameState.INPUT) Input(dT);
            else if (State == GameState.UPDATE) Update(dT); // TODO :((( Viel Spaß beim Client / Server separieren
            else if (State == GameState.PAUSE) Pause(dT);
            else if (State == GameState.GAMESTART) GameStart(dT);
        }

        private void Input(float dT)
        {
            InputTimeLeft -= dT;
            if (InputTimeLeft <= 0)
            {
                if (Window.Server != null)
                {
                    SetState(GameState.UPDATE);
                }
                else
                {
                    State = GameState.PAUSE;
                }
                firstUpdateTick = true;
            }
        }

        public void SetState(GameState state)
        {
            if (state == GameState.UPDATE)
            {
                State = GameState.UPDATE;
                ownUpdateDone = false;
            }
            else
            {
                State = state;
            }
        }

        public Dictionary<byte, bool> ClientsFinishedUpdate = [];
        private bool ownUpdateDone = false;
        private bool confirmedUpdateToServer;

        private void Update(float dT)
        {
            if (!ownUpdateDone)
            {
                confirmedUpdateToServer = false;
                CurrentStage.Tick(dT, firstUpdateTick, out bool done);
                if (done)
                {
                    ownUpdateDone = true;
                    Console.WriteLine("Own update done, waiting for others");
                }
            }
            else
            {
                if (Window.Server != null)
                {
                    if (ClientsFinishedUpdate.Keys.Count == Window.Server.playerCount - 1)
                    {
                        State = GameState.INPUT;
                        InputTimeLeft = InputPhaseLength;
                        ServerMessages.SetGameState(Window.Server.bWriter, Client.Game.State, Client.Game.InputTimeLeft, CurrentStage.Round);
                        Console.WriteLine($"All Clients finished UPDATE, moving to INPUT ({ClientsFinishedUpdate.Keys.Count})");
                        ClientsFinishedUpdate = [];
                    }
                }
                else if (!confirmedUpdateToServer)
                {
                    ClientMessages.UpdateDone(Window.Client.writer, CurrentStage.Round);
                    State = GameState.PAUSE;
                    confirmedUpdateToServer = true;
                }
            }

            firstUpdateTick = false;
        }

        public void Pause(float dT)
        {

        }

        public Dictionary<byte, bool> ClientsReceivedStart = [];
        public void GameStart(float dT)
        {
            if (Window.Server != null)
            {
                if (ClientsReceivedStart.Keys.Count == Window.Server.playerCount - 1)
                {
                    Console.WriteLine($"All Clients ready to start Game.");
                    ClientsReceivedStart = [];
                    State = GameState.INPUT;
                    ServerMessages.SetGameState(Window.Server.bWriter, State, 0, 0);
                }
            }
        }
    }
}