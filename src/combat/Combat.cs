namespace fiveSeconds
{
    public interface ICombat
    {   
        public Stats BaseStats {get; set;}
        public Stats Stats {get; set;}
        public DamageModifiers DamageDealAdds {get; set;}
        public DamageModifiers DamageDealMults {get; set;}
        public DamageModifiers DamageTakeAdds {get; set;}
        public DamageModifiers DamageTakeMults {get; set;}
        public StatusEffects StatusEffects {get; set;}
    }


}