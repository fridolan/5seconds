namespace fiveSeconds
{
    public class Aspect : Entity, ICombat
    {
        public Stats BaseStats { get; set ; }
        public Stats Stats { get; set; }

        public Aspect()
        {
            AtlasIndex = 1;
        }
    }
}