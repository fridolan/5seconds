using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class DamageType
    {
        public string Name;
        public Color4 Color;

        public static readonly DamageType FIRE = new()
        {
            Name = "Fire",
            Color = new(240, 58, 3, 1),
        };

        public static readonly DamageType FROST = new()
        {
            Name = "Frost",
            Color = new(57, 224, 244, 1),
        };

        public static readonly DamageType LIGHTNING = new()
        {
            Name = "Lightning",
            Color = new(79, 26, 122, 1),
        };

        public static readonly DamageType POISON = new()
        {
            Name = "Poison",
            Color = new(26, 95, 26, 1),
        };

        public static readonly DamageType MELEE = new()
        {
            Name = "Melee",
            Color = new(150, 150, 150, 1),
        };

        public static readonly DamageType RANGED = new()
        {
            Name = "Ranged",
            Color = new(0, 255, 0, 1),
        };

        public static readonly DamageType HEAL = new()
        {
            Name = "Heal",
            Color = new(0, 255, 0, 1),
        };
    }
}