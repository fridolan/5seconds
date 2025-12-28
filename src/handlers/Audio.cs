using System.Runtime.InteropServices;
using NAudio.Wave;
using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace fiveSeconds
{

    public enum Sounds
    {
        chopinop28no15,
        chopinop48no1,
        klack,
        mondscheinsonate1,
        mondscheinsonate2,
        monk,
        steps1,
        steps2,
        ungarischerTanz,
        wind
    }

    public class AudioManager

    {

        [DllImport("openal32.dll", EntryPoint = "alGenFilters", ExactSpelling = true)]
        public static extern void AlGenFilters(int n, out int filters);

        [DllImport("openal32.dll", EntryPoint = "alFilteri", ExactSpelling = true)]
        public static extern void AlFilteri(int filter, int param, int value);

        [DllImport("openal32.dll", EntryPoint = "alFilterf", ExactSpelling = true)]
        public static extern void AlFilterf(int filter, int param, float value);

        // Konstanten aus EFX.h (OpenAL Soft)
        public const int AL_FILTER_TYPE = 0x8001;
        public const int AL_FILTER_LOWPASS = 0x0001;
        public const int AL_LOWPASS_GAINHF = 0x0003;

        public static ALDevice device;
        public static ALContext context;

        //private static readonly Dictionary<string, int> soundBuffers = new(); // Datei -> Buffer
        public static readonly List<SoundInstance> activeSounds = new();
        private static readonly Queue<int> sourcePool = new();
        private static readonly List<int> soundBuffers = new();

        public static readonly Dictionary<int, float> bufferDurations = new();

        private const int MaxSources = 32; // OpenAL ist oft auf 32 limitiert

        private bool efxSupported;

        public AudioManager()
        {
            device = ALC.OpenDevice(null); // Default device
            if (device == ALDevice.Null)
                throw new Exception("OpenAL Gerät konnte nicht geöffnet werden.");

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            context = ALC.CreateContext(device, (int[])null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (context == ALContext.Null)
                throw new Exception("OpenAL Kontext konnte nicht erstellt werden.");

            ALC.MakeContextCurrent(context);

            var current = ALC.GetCurrentContext();
            if (current != context)
                throw new Exception("OpenAL-Kontext ist nicht aktiv.");

            string actualDeviceName = ALC.GetString(device, AlcGetString.DeviceSpecifier);
            Console.WriteLine("Tatsächlich geöffnetes Gerät: " + actualDeviceName);

            AL.Listener(ALListener3f.Position, 0f, 0f, 0f);
            AL.Listener(ALListener3f.Velocity, 0f, 0f, 0f);
            AL.Listener(ALListenerfv.Orientation, [0f, 0f, -1f, 0f, 1f, 0f]);

            // Quellen vorbereiten
            for (int i = 0; i < MaxSources; i++)
                sourcePool.Enqueue(AL.GenSource());

            efxSupported = ALC.IsExtensionPresent(AudioManager.device, "ALC_EXT_EFX");
            Console.WriteLine("EFX supported: " + efxSupported);

            var error = AL.GetError();
            if (error != ALError.NoError) Console.WriteLine($"AL Error AudioManager {error}");
        }

        public static void LoadSound(string filePath)
        {
            /* if (soundBuffers.ContainsKey(filePath))
                return; */

            using var mp3Reader = new Mp3FileReader(filePath);
            using var pcmStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader);
            using var memStream = new MemoryStream();
            pcmStream.CopyTo(memStream);
            byte[] pcmData = memStream.ToArray();

            int channels = pcmStream.WaveFormat.Channels;
            int sampleRate = pcmStream.WaveFormat.SampleRate;

            ALFormat format = channels switch
            {
                1 => ALFormat.Mono16,
                2 => ALFormat.Stereo16,
                _ => throw new NotSupportedException("Nur Mono/Stereo MP3s unterstützt.")
            };

            int buffer = AL.GenBuffer();

            IntPtr unmanagedPtr = Marshal.AllocHGlobal(pcmData.Length);
            try
            {
                Marshal.Copy(pcmData, 0, unmanagedPtr, pcmData.Length);
                AL.BufferData(buffer, format, unmanagedPtr, pcmData.Length, sampleRate);
            }
            finally
            {
                Marshal.FreeHGlobal(unmanagedPtr);
            }

            float totalSeconds = (float)pcmData.Length / (channels * (pcmStream.WaveFormat.BitsPerSample / 8) * sampleRate);
            bufferDurations[buffer] = totalSeconds;

            //soundBuffers[filePath] = buffer;
            soundBuffers.Add(buffer);

            var error = AL.GetError();
            if (error != ALError.NoError) Console.WriteLine($"AL Error LoadSound {error}");
        }

        public static SoundInstance? PlaySound(int sound, Func<double, Vector2> positionFunc, float gain = 1.0f)
        {
            /* if (!soundBuffers.ContainsKey(filePath))
                throw new Exception($"Sound '{filePath}' nicht geladen."); */

            if (sourcePool.Count == 0)
                return null;
            int source = sourcePool.Dequeue();
            //int buffer = soundBuffers[filePath];
            int buffer = soundBuffers[sound];

            AL.Source(source, ALSourcei.Buffer, buffer);
            AL.Source(source, ALSourcef.Gain, gain);
            AL.Source(source, ALSourceb.Looping, false);

            Vector2 pos = positionFunc(0);
            AL.Source(source, ALSource3f.Position, pos.X, pos.Y, 0);

            AlGenFilters(1, out int lowpassFilter);
            OpenALEfx.AlFilteri(lowpassFilter, OpenALEfx.AL_FILTER_TYPE, OpenALEfx.AL_FILTER_LOWPASS);
            OpenALEfx.AlFilterf(lowpassFilter, OpenALEfx.AL_LOWPASS_GAIN, 1.0f);    // Gesamtlautstärke
            OpenALEfx.AlFilterf(lowpassFilter, OpenALEfx.AL_LOWPASS_GAINHF, 0f);  // Höhen stark dämpfen
            OpenALEfx.AlSourcei(source, OpenALEfx.AL_DIRECT_FILTER, lowpassFilter);

            AL.SourcePlay(source);

            var error = AL.GetError();
            if (error != ALError.NoError)
                Console.WriteLine($"OpenAL Error: {error}");
            else
            {
                //Console.WriteLine("No error");
            }

            var instance = new SoundInstance(source, positionFunc, OnSoundFinished, lowpassFilter);
            activeSounds.Add(instance);
            return instance;
        }

        public static void Update(double time)
        {
            foreach (var sound in activeSounds.ToArray())
            {
                sound.Update(sound.Progress);

                int stateInt = AL.GetSource(sound.SourceId, ALGetSourcei.SourceState);
                var state = (ALSourceState)stateInt;
                if (state != ALSourceState.Playing)
                {
                    StopAndRecycle(sound);
                }
            }
        }

        private static void StopAndRecycle(SoundInstance instance)
        {
            var error1 = AL.GetError();
            AL.SourceStop(instance.SourceId);
            AL.Source(instance.SourceId, ALSourcei.Buffer, 0); // Detach
            int lowpassFilter = instance.FilterId;
            OpenALEfx.AlDeleteFilters(1, ref lowpassFilter);
            sourcePool.Enqueue(instance.SourceId);
            activeSounds.Remove(instance);

            var error = AL.GetError();
            if (error != ALError.NoError && error !=ALError.InvalidName) Console.WriteLine($"AL Error StopAndRecyle {error}");
        }

        private static void OnSoundFinished(SoundInstance instance)
        {
            StopAndRecycle(instance);
        }

        public static void Dispose()
        {
            foreach (var s in activeSounds)
            {
                AL.SourceStop(s.SourceId);
                AL.DeleteSource(s.SourceId);
            }

            foreach (var s in sourcePool)
                AL.DeleteSource(s);

            foreach (var b in soundBuffers/* .Values */)
                AL.DeleteBuffer(b);
        }

        public static void LoadAllSoundsFromDirectory()
        {
            foreach (var file in Directory.EnumerateFiles("sounds/", "*.mp3", SearchOption.AllDirectories).OrderBy(f => f))
            {
                try
                {
                    LoadSound(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Laden von '{file}': {ex.Message}");
                }
            }
        }
    }

}