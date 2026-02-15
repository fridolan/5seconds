using LiteNetLib.Utils;

namespace fiveSeconds
{
    public abstract class AbilityInput
    {
        public bool Wait { get; set; }
        public bool Complete { get; set; } = false;
        public bool Updating { get; set; } = true;

        public abstract void HandleAbilityInput(AbilityContext context);

        public static AbilityInput FromReader(NetDataReader reader)
        {
            int typeIndex = reader.GetInt();
            AbilityInput abilityInput = GetInstance[typeIndex]();

            if (abilityInput is IInputStartGoal sgi)
            {
                sgi.Start = (reader.GetInt(), reader.GetInt());
                sgi.Goal = (reader.GetInt(), reader.GetInt());
            }

            if (abilityInput is IInputEntityID ei)
            {
                ei.EntityID = reader.GetInt();
            }

            if (abilityInput is IInputCancelOnDisplace cod)
            {
                cod.CancelOnDisplace = reader.GetBool();
            }

            if (abilityInput is IInputRelative r)
            {
                r.Relative = reader.GetBool();
            }

            if (abilityInput is IInputToEntityID tei)
            {
                tei.ToEntityID = reader.GetInt();
            }

            return abilityInput;
        }

        public void Write(NetDataWriter writer)
        {
            AbilityInput abilityInput = this;

            writer.Put(GetTypeIndex[abilityInput.GetType()]);

            if (abilityInput is IInputStartGoal sgi)
            {
                writer.Put(sgi.Start.X);
                writer.Put(sgi.Start.Y);
                writer.Put(sgi.Goal.X);
                writer.Put(sgi.Goal.Y);
            }

            if (abilityInput is IInputEntityID ei)
            {
                writer.Put(ei.EntityID);
            }

            if (abilityInput is IInputCancelOnDisplace cod)
            {
                writer.Put(cod.CancelOnDisplace);
            }

            if (abilityInput is IInputRelative r)
            {
                writer.Put(r.Relative);
            }

            if (abilityInput is IInputToEntityID tei)
            {
                writer.Put(tei.ToEntityID);
            }

        }

        public static Dictionary<Type, int> GetTypeIndex = new(){
          { typeof(SourceTargetInput), 0 },
          { typeof(StartGoalInput), 1 },
          { typeof(AutoStartGoalInput), 2}
        };

        public static Dictionary<int, Func<AbilityInput>> GetInstance = new()
        {
            { 0, () => new SourceTargetInput()},
            { 1, () => new StartGoalInput()},
            { 2, () => new AutoStartGoalInput()},
        };
    }
}