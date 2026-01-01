using LiteNetLib.Utils;
using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class MoveEntityAction : SAction
    {
        public int EntityID;
        public bool CancelOnDisplace = true;
        public bool Relative = false;
        public int ToEntityID = -1;
        public Vector2i Start;
        public Vector2i Goal;
        public float TimePerStep = 0.5f;

        private int StepsTaken = 0;
        private Entity ToEntity;

        #region Activations
        public override void Begin()
        {
            Stage stage = Game.CurrentStage;
            Entity entity = Entity.GetByID(EntityID);
            if(entity == null) throw new Exception("MoveEntityAction no entity");

            if(Relative) Goal = Goal - Start + entity.Position;
            if(ToEntityID != -1)
            {
                ToEntity = Entity.GetByID(ToEntityID);
                if(ToEntity == null) throw new Exception("MoveEntityAction no toEntity"); 
                Goal = ToEntity.Position;
            }

            List<Vector2i> path = stage.GetPathTo(EntityID, Goal);
            Vector2i startingPosition = path[0];

            stage.MoveEntity(EntityID, path[0]);
            NextActivationTime = (StepsTaken + 1) * TimePerStep;
            NextActivation = TakeStep;
        }

        private void TakeStep()
        {
            //Console.WriteLine($"TakeStep {Path[NextStep]} {EntityID}");
            if(ToEntity != null) Goal = ToEntity.Position;
            List<Vector2i> path = Game.CurrentStage.GetPathTo(EntityID, Goal);
            if (path.Count < 2)
            {
                Finished = true;
                return;
            }
            Game.CurrentStage.MoveEntity(EntityID, path[1]);
            StepsTaken++;
            NextActivationTime = (StepsTaken + 1) * TimePerStep;

            if (path.Count <= 2)
            {
                Finished = true;
                return;
            }
        }
        #endregion

        #region Networking
        public static MoveEntityAction FromReader(NetDataReader reader)
        {
            MoveEntityAction action = new()
            {
                Start = (reader.GetInt(), reader.GetInt()),
                Goal = (reader.GetInt(), reader.GetInt()),
                TimePerStep = reader.GetFloat(),
                Relative = reader.GetBool(),
                EntityID = reader.GetInt(),
                CancelOnDisplace = reader.GetBool(),
            };

            return action;
        }

        public override void ToWriter(NetDataWriter writer)
        {
            writer.Put(Start.X); writer.Put(Start.Y);
            writer.Put(Goal.X); writer.Put(Goal.Y);
            writer.Put(TimePerStep);
            writer.Put(Relative);
            writer.Put(EntityID);
            writer.Put(CancelOnDisplace);
        }
        #endregion

    }
}