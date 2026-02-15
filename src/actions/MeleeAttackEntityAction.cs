using LiteNetLib.Utils;
using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class MeleeAttackEntityAbility : Ability
    {

        public override int Icon => Textures.sword;

        public int DamagePercent { get; set; } = 100;

        private Entity ToEntity;
        private Entity Entity;
        private float AttackTime = 1f;

        public MeleeAttackEntityAbility(Random random)
        {

        }

        #region Activations
        public override void Begin(AbilityAction action)
        {
            SourceTargetInput input = (SourceTargetInput)action.Input;

            Stage stage = Client.Game.CurrentStage;
            Entity = stage.GetEntityByID(input.EntityID) ?? throw new Exception($"MeleeAttackEntityAction no entity");
            if (Entity is not ICombat) throw new Exception("MeleeAttackEntityAction Entity not ICombat entity");

            if (input.ToEntityID == -1) throw new Exception("MeleeAttackEntityAction no ToEntityID");
            ToEntity = stage.GetEntityByID(input.ToEntityID) ?? throw new Exception("MeleeAttackEntityAction no toEntity");
            if (ToEntity is not ICombat) throw new Exception("MeleeAttackEntityAction ToEntity not ICombat entity");

            action.NextActivation = Attack;
        }

        private void Attack(AbilityAction action)
        {
            SourceTargetInput input = (SourceTargetInput)action.Input;

            Console.WriteLine($"Euc {(Entity.Position - ToEntity.Position).EuclideanLength}");
            if ((Entity.Position - ToEntity.Position).EuclideanLength < 2)
            {
                CombatContext combatContext = new()
                {
                    Damage = new()
                    {
                        Amount = 100 * DamagePercent / 100,
                        Type = DamageType.MELEE,
                    },
                    Source = (ICombat)Entity,
                    Target = (ICombat)ToEntity,
                };
                Damage.DamageC(combatContext);

                action.NextActivation = End;
                action.NextActivationTime += AttackTime;
            }
            else if (input.Wait)
            {
                action.Waiting = true;
            }
            else
            {
                action.NextActivation = End;
            }
        }
        #endregion

        private void End(AbilityAction action)
        {
            action.Finished = true;
        }

        public override AbilityInput GetNewAbilityInput() => new SourceTargetInput();
    }
}