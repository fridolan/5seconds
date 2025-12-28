using System.Runtime.InteropServices;

namespace fiveSeconds
{
    public static class OpenALEfx
    {
        // P/Invoke: Filter erzeugen/löschen
        [DllImport("openal32.dll", EntryPoint = "alGenFilters", ExactSpelling = true)]
        public static extern void AlGenFilters(int n, out int filters);

        [DllImport("openal32.dll", EntryPoint = "alDeleteFilters", ExactSpelling = true)]
        public static extern void AlDeleteFilters(int n, ref int filters);

        // Parameter setzen
        [DllImport("openal32.dll", EntryPoint = "alFilteri", ExactSpelling = true)]
        public static extern void AlFilteri(int filter, int param, int value);

        [DllImport("openal32.dll", EntryPoint = "alFilterf", ExactSpelling = true)]
        public static extern void AlFilterf(int filter, int param, float value);

        // Filter an Source binden
        [DllImport("openal32.dll", EntryPoint = "alSourcei", ExactSpelling = true)]
        public static extern void AlSourcei(int source, int param, int value);

        // Konstanten aus EFX.h
        public const int AL_FILTER_TYPE = 0x8001;
        public const int AL_FILTER_LOWPASS = 0x0001;

        public const int AL_LOWPASS_GAIN = 0x0001;    // Lautstärke gesamt (0.0 – 1.0)
        public const int AL_LOWPASS_GAINHF = 0x0002;  // Höhenanteil (0.0 – 1.0)

        public const int AL_DIRECT_FILTER = 0x20005;  // Source-Property für Direct-Filter
    }
}