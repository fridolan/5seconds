using LiteNetLib.Utils;

namespace fiveSeconds
{
    public class FireballAction : Ability
    {
        public override void Begin(AbilityAction action)
        {
            
        }

        public override AbilityInput GetNewAbilityInput() => new StartGoalInput();
    }
}