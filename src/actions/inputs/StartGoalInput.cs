using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class StartGoalInput : AbilityInput, IInputStartGoal, IInputEntityID, IInputCancelOnDisplace, IInputRelative
    {
        public Vector2i Start { get; set; }
        public Vector2i Goal { get; set; }
        public int EntityID { get; set; }
        public bool CancelOnDisplace { get; set; }
        public bool Relative { get; set; }

        public override void HandleAbilityInput(AbilityContext context)
        {
            throw new NotImplementedException();
        }
    }
}