using LiteNetLib.Utils;
using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class MoveEntityAbility : Ability
    {
        public override int Icon => Textures.move;


        private float TimePerStep = 0.5f;
        private int StepsTaken = 0;

        #region Activations
        public override void Begin(AbilityAction action)
        {
            AutoStartGoalInput input = (AutoStartGoalInput)action.Input;
            
            Stage stage = Client.Game.CurrentStage;
            Entity entity = stage.GetEntityByID(input.EntityID);
            if (entity == null) throw new Exception("MoveEntityAction no entity");

            if (input.Relative) input.Goal = input.Goal - input.Start + entity.Position;

            StepsTaken = 0;

            List<Vector2i> path = stage.GetPathTo(input.EntityID, input.Goal);

            stage.MoveEntity(input.EntityID, path[0]);
            action.NextActivationTime = (StepsTaken + 1) * TimePerStep;
            action.NextActivation = TakeStep;
        }

        private void TakeStep(AbilityAction action)
        {
            AutoStartGoalInput input = (AutoStartGoalInput)action.Input;
            //Console.WriteLine($"TakeStep {Path[NextStep]} {EntityID}");
            List<Vector2i> path = Client.Game.CurrentStage.GetPathTo(input.EntityID, input.Goal);
            if (path.Count < 2)
            {
                action.Finished = true;
                return;
            }
            Client.Game.CurrentStage.MoveEntity(input.EntityID, path[1]);
            StepsTaken++;
            action.NextActivationTime = (StepsTaken + 1) * TimePerStep;

            if (path.Count <= 2)
            {
                action.Finished = true;
                return;
            }
        }

        public override AbilityInput GetNewAbilityInput() => new AutoStartGoalInput();


        #endregion
    }
}