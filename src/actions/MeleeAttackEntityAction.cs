using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class MeleeAttackEntityAction : SAction, IEntityIDInput, IToEntityIDInput, IWaitInput, IDamagePercentInput
    {
        public int EntityID { get; set; }
        public bool CancelOnDisplace { get; set; } = true;
        public int ToEntityID { get; set; } = -1;
        public bool Wait { get; set; } = false;
        public int DamagePercent {get; set;} = 1;

        private Entity ToEntity;
        private Entity Entity;
        private float AttackTime = 1f;

        #region Activations
        public override void Begin(Game game)
        {
            Stage stage = game.CurrentStage;
            Entity = stage.GetEntityByID(EntityID) ?? throw new Exception($"MeleeAttackEntityAction no entity");
            if (Entity is not ICombat) throw new Exception("MeleeAttackEntityAction Entity not ICombat entity");

            if (ToEntityID == -1) throw new Exception("MeleeAttackEntityAction no ToEntityID");
            ToEntity = stage.GetEntityByID(ToEntityID) ?? throw new Exception("MeleeAttackEntityAction no toEntity");
            if (ToEntity is not ICombat) throw new Exception("MeleeAttackEntityAction ToEntity not ICombat entity");

            NextActivation = Attack;
        }

        private void Attack(Game game)
        {
            if ((Entity.Position - ToEntity.Position).EuclideanLength < 2)
            {
                CombatContext combatContext = new()
                {
                    Damage = new()
                    {
                        Amount = 100,
                        Type = DamageType.MELEE,
                    },
                    Source = (ICombat)Entity,
                    Target = (ICombat)ToEntity,
                };
                Damage.DamageC(combatContext);

                NextActivation = End;
                NextActivationTime += AttackTime;
            }
            else if (Wait)
            {
                Waiting = true;
            }
            else
            {
                NextActivation = End;
            }
        }
        #endregion

        private void End(Game _)
        {
            Finished = true;
        }
    }
}