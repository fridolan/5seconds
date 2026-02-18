using LiteNetLib.Utils;
using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class CatchEntityAbility : Ability
    {

        private long TimePerStep = 1_000_000 / 2;

        #region Activations
        public override void Begin(AbilityAction action)
        {
            SourceTargetInput input = (SourceTargetInput)action.Input;

            Stage stage = Client.Game.CurrentStage;
            Entity entity = stage.GetEntityByID(input.EntityID) ?? throw new Exception("CatchEntityAction no entity");
            
            if (input.ToEntityID == -1) throw new Exception("CatchEntityAction no ToEntityID");
            action.ToEntity = stage.GetEntityByID(input.ToEntityID) ?? throw new Exception("CatchEntityAction no toEntity");

            action.StepsTaken = 0;

            action.NextActivationTime = 1 * TimePerStep;
            action.NextActivation = TakeStep;
        }


        private void TakeStep(AbilityAction action)
        {
            SourceTargetInput input = (SourceTargetInput)action.Input;
            //Console.WriteLine($"TakeStep {Path[NextStep]} {EntityID}");
            List<Vector2i> path = Client.Game.CurrentStage.GetPathTo(input.EntityID, action.ToEntity.Position);
            if (path.Count < 3)
            {
                action.Finished = true;
                return;
            }
            Client.Game.CurrentStage.MoveEntity(input.EntityID, path[1]);
            action.StepsTaken++;
            action.NextActivationTime = (action.StepsTaken + 1) * TimePerStep;

            if (path.Count <= 3)
            {
                action.Finished = true;
                return;
            }
        }
        
        public override AbilityInput GetNewAbilityInput() => new SourceTargetInput();

        #endregion
    }
}