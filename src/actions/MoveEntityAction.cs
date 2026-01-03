using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class MoveEntityAction : SAction, IStartGoalInput, IEntityIDInput, ICancelOnDisplaceInput, IRelativeInput, IToEntityIDInput
    {
        public int EntityID {get; set;}
        public bool CancelOnDisplace {get; set;} = true;
        public bool Relative {get; set;} = false;
        public int ToEntityID {get; set;} = -1;
        public Vector2i Start { get; set; }
        public Vector2i Goal { get; set; }

        private float TimePerStep = 0.5f;
        private int StepsTaken = 0;
        private Entity ToEntity;

        #region Activations
        public override void Begin(Game game)
        {
            Stage stage = game.CurrentStage;
            Entity entity = stage.GetEntityByID(EntityID);
            if (entity == null) throw new Exception("MoveEntityAction no entity");

            if (Relative) Goal = Goal - Start + entity.Position;
            if (ToEntityID != -1)
            {
                ToEntity = stage.GetEntityByID(ToEntityID);
                if (ToEntity == null) throw new Exception("MoveEntityAction no toEntity");
                Goal = ToEntity.Position;
            }

            List<Vector2i> path = stage.GetPathTo(EntityID, Goal);
            Vector2i startingPosition = path[0];

            stage.MoveEntity(EntityID, path[0]);
            NextActivationTime = (StepsTaken + 1) * TimePerStep;
            NextActivation = TakeStep;
        }

        private void TakeStep(Game game)
        {
            //Console.WriteLine($"TakeStep {Path[NextStep]} {EntityID}");
            if (ToEntity != null) Goal = ToEntity.Position;
            List<Vector2i> path = game.CurrentStage.GetPathTo(EntityID, Goal);
            if (path.Count < 2)
            {
                Finished = true;
                return;
            }
            game.CurrentStage.MoveEntity(EntityID, path[1]);
            StepsTaken++;
            NextActivationTime = (StepsTaken + 1) * TimePerStep;

            if (path.Count <= 2)
            {
                Finished = true;
                return;
            }
        }
        #endregion
    }
}