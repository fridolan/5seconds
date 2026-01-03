namespace fiveSeconds
{
    public class StatusEffect
    {

        public int Index;

        public static readonly StatusEffect Freezing = new()
        {
            Index = 0,
        };

        public static readonly StatusEffect Burning = new()
        {
            Index = 1,
        };

        public static readonly StatusEffect Wet = new()
        {
            Index = 2,
        };

        public static readonly StatusEffect Electrocuted = new()
        {
            Index = 3,
        };

        public const int TypeCount = 4;

        public static implicit operator int(StatusEffect type)
        {
            return type.Index;
        }

        public static int[] FilledArray(int value)
        {
            int[] array = new int[TypeCount];
            for (int i = 0; i < array.Length; i++)
                array[i] = value;

            return array;
        }
    }
}