using LiteNetLib.Utils;

namespace fiveSeconds
{
    public class PlaceholderAbility : Ability
    {
        public override void Begin(AbilityAction action)
        {
            throw new NotImplementedException();
        }

        public override AbilityInput GetNewAbilityInput() => new StartGoalInput();
    }
}