
namespace fiveSeconds
{
    public class Aspect : Entity, ICombat
    {
        public Stats BaseStats { get; set ; }
        public Stats Stats { get; set; }
        public List<Ability> Abilities { get; set; }

        public Aspect()
        {
            AtlasIndex = 1;
            Stats = new();
            BaseStats = new();
            Abilities = [
                new MoveEntityAbility(),
                new CatchEntityAbility(),
                new MeleeAttackEntityAbility(Client.Game.Random),
                new FireballAction(Client.Game.Random),
            ];
        }
    }
}