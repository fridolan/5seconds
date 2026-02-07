namespace fiveSeconds
{
    public abstract class Ability
    {
        public string Name;
        public SAction Action;
        public int TimeCost;

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

            ability.U(abilityContext);

            return null;
        }

        public abstract void U(AbilityContext abilityContext);
    }
}