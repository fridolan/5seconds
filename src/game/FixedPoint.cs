using OpenTK.Mathematics;

namespace fiveSeconds
{
    public static class FP
    {
        public const int FRACTION_BITS = 16;
        public const int SCALE = 1 << FRACTION_BITS;

        public static int Mult(int a, int b)
        {
            return (int)(((long)a * b) >> FRACTION_BITS);
        }

        public static Vector2i Mult(Vector2i v, int b)
        {
            return ((int)(((long)v.X * b) >> FRACTION_BITS),
            (int)(((long)v.Y * b) >> FRACTION_BITS));
        }

        public static int Div(int a, int b)
        {
            return (int)(((long)a << FRACTION_BITS) / b);
        }

        public static Vector2i Div(Vector2i v, int b)
        {
            return ((int)(((long)v.X << FRACTION_BITS) / b),
            (int)(((long)v.X << FRACTION_BITS) / b));
        }

        public static int LengthSq(int a, int b)
        {
            return (int)(((long)a * a + (long)b * b) >> FRACTION_BITS);
        }

        public static Vector2i Normalize(Vector2i v, out int lengthFixed)
        {
            int x = v.X;
            int y = v.Y;
            long lengthSq = (long)x * x + (long)y * y;
            long length = IntSqrt(lengthSq);

            if (length == 0)
            {
                lengthFixed = 0;
                return (0, 0);
            }

            lengthFixed = (int)length;

            return ((int)(((long)x << FRACTION_BITS) / length),
            (int)(((long)y << FRACTION_BITS) / length));
        }

        public static long IntSqrt(long n)
        {
            if (n <= 0) return 0;

            long x = n;
            long y = (x + 1) >> 1;

            while (y < x)
            {
                x = y;
                y = (x + n / x) >> 1;
            }

            return x;
        }

        public static Vector2i ToFixed(Vector2i v)
        {
            return (v.X << FRACTION_BITS, v.Y << FRACTION_BITS);
        }

    }
}