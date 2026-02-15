using LiteNetLib.Utils;

namespace fiveSeconds
{

    public abstract class Ability
    {
        public virtual int Icon => 0;

        public int ID;

        public int Cooldown;
        public int LastUsed;
        public int ManaCost;
        public int TimeCost;
        public string Name;

        public Type InputType;

        /*       public abstract void InitAbilityInput();
              public abstract void SubmitAbilityInput(Entity entity);
              public abstract void CancelAbilityInput();
              public abstract void HandleAbilityInput(AbilityContext context);
              public abstract void WriteInputs(NetDataWriter writer);
              public abstract void ReadInputs(NetDataReader reader); */

        public Ability()
        {
            ID = Client.Game.AbilityIDCounter;
            Client.Game.AbilityIDCounter++;
            Client.Game.Abilities.Add(this);
        }

        public abstract void Begin(AbilityAction action);

        public static Ability ExistingFromReader(NetDataReader reader)
        {

            int abilityID = reader.GetInt();

            Ability ability = Client.Game.Abilities.Find(a => a.ID == abilityID);
            if (ability == null) throw new Exception($"ActivationFromReader AbilityID {abilityID} not found in Game.Abilities");
            return ability;
        }

        public abstract AbilityInput GetNewAbilityInput();

        // public void InitAbilityInput() => Inputs.Add(new());
        // public void CancelAbilityInput() => Inputs.RemoveAt(Inputs.Count - 1);
        /* public void SubmitAbilityInput(Entity entity)
        {
            Console.WriteLine($"Submitting {Inputs[^1].Complete} {Inputs.Count}");
            if (Inputs[^1].Complete)
            {
                entity.AddAction(this);
                Inputs[^1].Updating = false;
            }
            else CancelAbilityInput();
        }
        public override void HandleAbilityInput(AbilityContext context)
        {
            if (Inputs[^1].Updating) Inputs[^1].HandleAbilityInput(context);
        } */

        public static Dictionary<Type, int> GetTypeIndex = new(){
          { typeof(MoveEntityAbility), 0 },
          { typeof(CatchEntityAbility), 1 },
          { typeof(MeleeAttackEntityAbility), 2 },
        };

        /*  public static List<Func<Ability>> GetInstance = [
             () => new MoveEntityAbility(),
             () => new CatchEntityAbility(),
             () => new MeleeAttackEntityAbility(),
         ]; */
    }
}