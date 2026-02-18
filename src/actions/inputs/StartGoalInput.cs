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
            Start = context.SourceEntity.Position;
            EntityID = context.SourceEntity.ID;
            if (context.ValidHover)
            {
                Goal = context.HoveredTile;
                Complete = true;
            }
        }
    }
}