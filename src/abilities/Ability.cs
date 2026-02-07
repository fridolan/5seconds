namespace fiveSeconds
{
    public abstract class Ability
    {
        public string Name;
        public SAction Action;
        public int TimeCost;
        public virtual int Icon => 0;

        public static Type? Use(Ability ability, AbilityContext abilityContext)
        {
            if(ability is IAbilityCooldown c)
            {
                if (abilityContext.Stage.Round < c.Cooldown + c.LastUsed)
                {
                    return typeof(IAbilityCooldown);
                }
                
                c.LastUsed = abilityContext.Stage.Round;
            }

            if(ability is IAbilityManaCost m)
            {
                if(abilityContext.SourceEntity is ICombat combatEntity)
                {
                    bool result = combatEntity.Stats.CostMana(m.ManaCost);
                    if(!result) return typeof(IAbilityManaCost);
                } else return typeof(IAbilityManaCost);
            }

            if(ability is IAbilityTargetRequirement tr)
            {
                if(abilityContext.TargetEntity == null) return typeof(IAbilityTargetRequirement);
            }

            ability.U(abilityContext);

            return null;
        }

        public abstract bool U(AbilityContext abilityContext);
    }
}