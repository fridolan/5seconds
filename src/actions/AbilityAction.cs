using LiteNetLib.Utils;

namespace fiveSeconds
{
    public class AbilityAction
    {
        public Ability Ability;
        public AbilityInput Input;

        public float TimePassed = 0;
        public float NextActivationTime = 0;
        public Action<AbilityAction> NextActivation;
        public bool Finished = false;
        public bool Begun = false;
        public bool Waiting = false;

        public void Execute()
        {
            float now = NextActivationTime;
            if (Begun == false)
            {
                Ability.Begin(this);
                Begun = true;
            }
            else NextActivation(this);
            TimePassed = now;
        }

        public void Write(NetDataWriter writer)
        {
            writer.Put(Ability.ID);
            Input.Write(writer);
        }

        public static AbilityAction FromReader(NetDataReader reader)
        {
            AbilityAction action = new();
            int abilityID = reader.GetInt();
            action.Ability = Client.Game.Abilities.Find(a => a.ID == abilityID);
            action.Input = AbilityInput.FromReader(reader);

            return action;
        }
    }
}