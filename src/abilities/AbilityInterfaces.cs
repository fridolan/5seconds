namespace fiveSeconds
{
    public interface IAbilityManaCost
    {
        public int ManaCost {get; set;}
    }

    public interface IAbilityDamage
    {
        public int Damage {get; set;}
    }

    public interface IAbilityCooldown
    {
        public int Cooldown {get; set;}
        public int LastUsed {get; set;}
    }

    public interface IAbilityTargetRequirement
    {
        
    }

}