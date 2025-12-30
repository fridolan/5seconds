using LiteNetLib.Utils;
using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class MoveEntityAction : SAction
    {
        public int EntityID;
        public bool CancelOnDisplace = true;
        public List<Vector2i> Path = [];
        public float TotalTime;

        public int StepCount => Path.Count - 1;
        public float TimePerStep => TotalTime / StepCount;
        public int NextStep = 0;

        #region Activations
        public override void Begin()
        {
            Stage stage = Game.CurrentStage;
            Vector2i startingPosition = Path[0];
            
            Game.CurrentStage.MoveEntity(EntityID, Path[0]);

            NextStep = 1;
            NextActivationTime = NextStep * TimePerStep;
            NextActivation = TakeStep;
        }

        private void TakeStep()
        {
            //Console.WriteLine($"TakeStep {Path[NextStep]} {EntityID}");
            Game.CurrentStage.MoveEntity(EntityID, Path[NextStep]);
            NextStep++;
            NextActivationTime = NextStep * TimePerStep;

            if (NextStep >= Path.Count) Finished = true;
        }
        #endregion

        #region Networking
        public static MoveEntityAction FromReader(NetDataReader reader)
        {
            int pathLength = reader.GetInt();
            List<Vector2i> path = [];
            for (int i = 0; i < pathLength; i++)
            {
                Vector2i pos = (reader.GetInt(), reader.GetInt());
                path.Add(pos);
            }
            MoveEntityAction action = new()
            {
                EntityID = reader.GetInt(),
                CancelOnDisplace = reader.GetBool(),
                TotalTime = reader.GetInt(),
                Path = path,
            };

            return action;
        }

        public override void ToWriter(NetDataWriter writer)
        {
            writer.Put(Path.Count);
            for (int i = 0; i < Path.Count; i++)
            {
                Vector2i pos = Path[i];
                writer.Put(pos.X);
                writer.Put(pos.Y);
            }
            writer.Put(EntityID);
            writer.Put(CancelOnDisplace);
            writer.Put(TotalTime);
        }
        #endregion

    }
}