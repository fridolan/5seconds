namespace fiveSeconds
{
    public class MeleeAttackAbility : Ability, IAbilityDamage
    {
        public int Damage { get; set; } = 1;
        public override int Icon => Textures.sword;

        public override bool U(AbilityContext context)
        {
            if(context.TargetEntity is Entity target){
            MeleeAttackEntityAction action = new()
            {
                EntityID = context.SourceEntity.ID,
                ToEntityID = target.ID,
                DamagePercent = Damage,
            };

            context.SourceEntity.AddAction(action);
            return true;
            } else return false;
        }
    }
}