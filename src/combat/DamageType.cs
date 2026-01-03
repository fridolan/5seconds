using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class DamageType
    {
        public string Name;
        public Color4 Color;
        public int Index;

        public static readonly DamageType FIRE = new()
        {
            Name = "Fire",
            Color = new(240, 58, 3, 1),
            Index = 0,
        };

        public static readonly DamageType FROST = new()
        {
            Name = "Frost",
            Color = new(57, 224, 244, 1),
            Index = 1,
        };

        public static readonly DamageType LIGHTNING = new()
        {
            Name = "Lightning",
            Color = new(79, 26, 122, 1),
            Index = 2,
        };

        public static readonly DamageType POISON = new()
        {
            Name = "Poison",
            Color = new(26, 95, 26, 1),
            Index = 3,
        };

        public static readonly DamageType MELEE = new()
        {
            Name = "Melee",
            Color = new(150, 150, 150, 1),
            Index = 4,
        };

        public static readonly DamageType RANGED = new()
        {
            Name = "Ranged",
            Color = new(0, 255, 0, 1),
            Index = 5,
        };

        public static readonly DamageType HEAL = new()
        {
            Name = "Heal",
            Color = new(0, 255, 0, 1),
            Index = 6,
        };

        public const int TypeCount = 7;

        public static implicit operator int(DamageType type)
        {
            return type.Index;
        }

        public static float[] FilledArray(float value)
        {
            float[] array = new float[TypeCount];
            for (int i = 0; i < array.Length; i++)
                array[i] = value;

            return array;
        }

    }
}