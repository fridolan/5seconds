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

        public static void OnLoad()
        {

        }

        public static void OnUpdateFrame(FrameEventArgs args)
        {
            float dT = (float)args.Time;
            if (State == GameState.INPUT) Input(dT);
            else if (State == GameState.UPDATE) Update(dT);
            else if (State == GameState.PAUSE) Pause(dT);
        }

        private static void Input(float dT)
        {
            InputTimeLeft -= dT;
            if (InputTimeLeft <= 0)
            {
                State = GameState.UPDATE;
                return;
            }
        }

        private static void Update(float dT)
        {
            CurrentStage.Tick(dT, out bool done);
            if(done) {
                State = GameState.INPUT;
                InputTimeLeft = InputPhaseLength;
            }
        }

        public static void Pause(float dT)
        {
            
        }
    }
}