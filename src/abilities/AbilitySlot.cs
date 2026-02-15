using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class AbilitySlot
    {
        public Vector2 Position;
        public Vector2 Size;
        public Ability Ability;

        public static AbilitySlot[] InitArray()
        {
            AbilitySlot[] array = new AbilitySlot[10];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new AbilitySlot()
                {
                    
                };
            }

            return array;
        }
    }
}