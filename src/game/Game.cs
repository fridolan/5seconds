using OpenTK.Windowing.Common;

namespace fiveSeconds
{
    public enum GameState
    {
        INPUT,
        UPDATE,
        PAUSE
    }

    public class Game
    {
        public GameState State;
        public Random Random = new();
        public Stage CurrentStage;

        public bool ManuallyPaused = false;
        public float InputTimeLeft = 0;
        public float InputPhaseLength = 6;

        private bool firstUpdateTick = false;

        public void OnLoad()
        {
            CurrentStage = new Cave1()
            {
                Game = this,
            };
        }

        public void OnUpdateFrame(FrameEventArgs args)
        {
            float dT = (float)args.Time;
            if (State == GameState.INPUT) Input(dT);
            else if (State == GameState.UPDATE) Update(dT); // TODO :((( Viel Spaß beim Client / Server separieren
            else if (State == GameState.PAUSE) Pause(dT);
        }

        private void Input(float dT)
        {
            InputTimeLeft -= dT;
            if (InputTimeLeft <= 0)
            {
                firstUpdateTick = true;
                State = GameState.UPDATE;
                return;
            }
        }

        private void Update(float dT)
        {
            CurrentStage.Tick(dT, firstUpdateTick, out bool done);
            if (done)
            {
                State = GameState.INPUT;
                InputTimeLeft = InputPhaseLength;
            }
            firstUpdateTick = false;
        }

        public void Pause(float dT)
        {

        }
    }
}