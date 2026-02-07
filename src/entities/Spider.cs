namespace fiveSeconds
{
    public class Spider : Entity, ICombat
    {
        public Stats BaseStats { get; set ; }
        public Stats Stats { get; set; }
        public List<Ability> Abilities {get; set;}
        
        public Spider()
        {
            AtlasIndex = 0;
            Stats = new();
            BaseStats = new();
            Abilities = [
                new MeleeAttackAbility(),
            ];
        }
    }
}