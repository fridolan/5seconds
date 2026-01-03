using OpenTK.Windowing.Common;

namespace fiveSeconds
{
    public enum GameState
    {
        INPUT,
        UPDATE,
        PAUSE
    }

    public static class Game
    {
        public static GameState State;
        public static Random Random = new();
        public static Stage CurrentStage = new Cave1();

        public static bool ManuallyPaused = false;
        public static float InputTimeLeft = 0;

        private static float InputPhaseLength = 6;

        private static bool firstUpdateTick = false;

        public static void OnLoad()
        {

        }

        public static void OnUpdateFrame(FrameEventArgs args)
        {
            float dT = (float)args.Time;
            if (State == GameState.INPUT) Input(dT);
            else if (State == GameState.UPDATE) Update(dT); // TODO :((( Viel Spaß beim Client / Server separieren
            else if (State == GameState.PAUSE) Pause(dT);
        }

        private static void Input(float dT)
        {
            InputTimeLeft -= dT;
            if (InputTimeLeft <= 0)
            {
                firstUpdateTick = true;
                State = GameState.UPDATE;
                return;
            }
        }

        private static void Update(float dT)
        {
            CurrentStage.Tick(dT, firstUpdateTick, out bool done);
            if(done) {
                State = GameState.INPUT;
                InputTimeLeft = InputPhaseLength;
            }
            firstUpdateTick = false;
        }

        public static void Pause(float dT)
        {
            
        }
    }
}