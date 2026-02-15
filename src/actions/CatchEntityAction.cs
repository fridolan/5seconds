using LiteNetLib.Utils;
using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class CatchEntityAbility : Ability
    {

        private float TimePerStep = 0.5f;
        private int StepsTaken = 0;
        private Entity ToEntity;

        #region Activations
        public override void Begin(AbilityAction action)
        {
            SourceTargetInput input = (SourceTargetInput)action.Input;

            Stage stage = Client.Game.CurrentStage;
            Entity entity = stage.GetEntityByID(input.EntityID) ?? throw new Exception("CatchEntityAction no entity");
            
            if (input.ToEntityID == -1) throw new Exception("CatchEntityAction no ToEntityID");
            ToEntity = stage.GetEntityByID(input.ToEntityID) ?? throw new Exception("CatchEntityAction no toEntity");

            StepsTaken = 0;

            action.NextActivationTime = 1 * TimePerStep;
            action.NextActivation = TakeStep;
        }


        private void TakeStep(AbilityAction action)
        {
            SourceTargetInput input = (SourceTargetInput)action.Input;
            //Console.WriteLine($"TakeStep {Path[NextStep]} {EntityID}");
            List<Vector2i> path = Client.Game.CurrentStage.GetPathTo(input.EntityID, ToEntity.Position);
            if (path.Count < 3)
            {
                action.Finished = true;
                return;
            }
            Client.Game.CurrentStage.MoveEntity(input.EntityID, path[1]);
            StepsTaken++;
            action.NextActivationTime = (StepsTaken + 1) * TimePerStep;

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