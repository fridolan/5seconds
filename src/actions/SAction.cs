using LiteNetLib.Utils;

namespace fiveSeconds
{
    public abstract class SAction
    {
        public float TimePassed = 0;
        public float NextActivationTime = 0;
        public Action NextActivation;
        public bool Finished = false;

        public void Execute()
        {
            float now = NextActivationTime;
            if (TimePassed == 0) Begin();
            else NextActivation();
            TimePassed = now;
        }

        public abstract void Begin();
        public abstract void ToWriter(NetDataWriter writer);
    }
}