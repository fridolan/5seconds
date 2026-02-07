using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class CatchEntityAction : SAction, IEntityIDInput, ICancelOnDisplaceInput, IToEntityIDInput
    {
        public int EntityID { get; set; }
        public bool CancelOnDisplace { get; set; } = true;
        public int ToEntityID { get; set; } = -1;

        private float TimePerStep = 0.5f;
        private int StepsTaken = 0;
        private Entity ToEntity;

        #region Activations
        public override void Begin(Game game)
        {
            Stage stage = game.CurrentStage;
            Entity entity = stage.GetEntityByID(EntityID) ?? throw new Exception("CatchEntityAction no entity");
            
            if (ToEntityID == -1) throw new Exception("CatchEntityAction no ToEntityID");
            ToEntity = stage.GetEntityByID(ToEntityID) ?? throw new Exception("CatchEntityAction no toEntity");

            NextActivationTime = 1 * TimePerStep;
            NextActivation = TakeStep;
        }

        private void TakeStep(Game game)
        {
            //Console.WriteLine($"TakeStep {Path[NextStep]} {EntityID}");
            List<Vector2i> path = game.CurrentStage.GetPathTo(EntityID, ToEntity.Position);
            if (path.Count < 3)
            {
                Finished = true;
                return;
            }
            game.CurrentStage.MoveEntity(EntityID, path[1]);
            StepsTaken++;
            NextActivationTime = (StepsTaken + 1) * TimePerStep;

            if (path.Count <= 3)
            {
                Finished = true;
                return;
            }
        }
        #endregion
    }
}