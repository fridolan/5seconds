namespace fiveSeconds
{
    public class SourceTargetInput : AbilityInput, IInputEntityID, IInputCancelOnDisplace, IInputToEntityID
    {
        public int EntityID { get; set; } = -1;
        public bool CancelOnDisplace { get; set; }
        public int ToEntityID { get; set; } = -1;

        public override void HandleAbilityInput(AbilityContext context)
        {
            EntityID = context.SourceEntity.ID;
            CancelOnDisplace = false;
            if (!(context.HoveredEntity == null && ToEntityID != -1))
            {
                ToEntityID = context.HoveredEntity?.ID ?? -1;
            }
            Complete = ToEntityID != -1;

        }
    }
}