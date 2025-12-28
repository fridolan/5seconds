using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace fiveSeconds
{

    public class SoundInstance
    {
        public int SourceId { get; }
        public int FilterId { get; }
        private readonly Func<double, Vector2> getPosition;
        private readonly Action<SoundInstance> onFinished;

        public SoundInstance(int sourceId, Func<double, Vector2> getPosition, Action<SoundInstance> onFinished, int filterId)
        {
            SourceId = sourceId;
            this.getPosition = getPosition;
            this.onFinished = onFinished;
            if (filterId != -1) FilterId = filterId;
        }

        public void Update(double time)
        {
            Vector2 pos = getPosition(time);
            AL.Source(SourceId, ALSource3f.Position, pos.X, pos.Y, 0);

            float dampening = 1;

            dampening = 0;

            // Console.WriteLine($"dampening {dampening} {solidPercent}");


            OpenALEfx.AlFilterf(FilterId, OpenALEfx.AL_LOWPASS_GAINHF, dampening);
            // AL.Source(SourceId, OpenALEfx.AL_DIRECT_FILTER, FilterId);
            OpenALEfx.AlSourcei(SourceId, OpenALEfx.AL_DIRECT_FILTER, FilterId);

            var error = AL.GetError();
            if (error != ALError.NoError) Console.WriteLine($"AL Error SoundInstanceUpdate {SourceId} {error}");
        }

        public void Stop()
        {
            onFinished?.Invoke(this);
        }

        public float Progress
        {
            get
            {
                int bufferId = AL.GetSource(SourceId, ALGetSourcei.Buffer);
                float secondsPlayed = AL.GetSource(SourceId, ALSourcef.SecOffset);

                 var error = AL.GetError();
            if (error != ALError.NoError) Console.WriteLine($"AL Error Progress {error}");

                if (AudioManager.bufferDurations.TryGetValue(bufferId, out float totalSec) && totalSec > 0)
                    return secondsPlayed / totalSec;
                else
                    return 0f;
            }
        }
    }

}