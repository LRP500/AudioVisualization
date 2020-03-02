using UnityEngine;

namespace AudioVisualization
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPeer : MonoBehaviour
    {
        public static readonly int SampleCount = 512;
        public static readonly int FrequencyBandCount = 8;

        public static float[] Samples { get; private set; } = null;
        public static float[] FrequencyBands { get; private set; } = null;
        public static float[] BandBuffers { get; private set; } = null;

        private AudioSource _audioSource = null;

        private float[] _bufferDecrease = null;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _bufferDecrease = new float[FrequencyBandCount];

            Samples = new float[SampleCount];
            FrequencyBands = new float[FrequencyBandCount];
            BandBuffers = new float[FrequencyBandCount];
        }

        private void Update()
        {
            GetSpectrumAudioSource();
            CreateFrequencyBands();
            BandBuffer();
        }

        private void BandBuffer()
        {
            for (int i = 0; i < FrequencyBandCount; i++)
            {
                if (FrequencyBands[i] > BandBuffers[i])
                {
                    BandBuffers[i] = FrequencyBands[i];
                    _bufferDecrease[i] = 0.005f;
                }

                if (FrequencyBands[i] < BandBuffers[i])
                {
                    BandBuffers[i] -= _bufferDecrease[i];
                    _bufferDecrease[i] *= 1.2f;
                }
            }
        }

        private void GetSpectrumAudioSource()
        {
            _audioSource.GetSpectrumData(Samples, 0, FFTWindow.Blackman);
        }

        private void CreateFrequencyBands()
        {
            /// 22050 / 512 = 43Hz per sample
            /// 20-60Hz
            /// 60-250Hz
            /// 250-500Hz
            /// 500-2000Hz
            /// 2000-4000Hz
            /// 4000-6000Hz
            /// 6000-20000Hz
            /// 0 - 2 samples = 86Hz
            /// 1 - 4 samples = 172Hz (87-258Hz)
            /// 2 - 8 samples = 344Hz (259-602Hz)
            /// 3 - 16 samples = 688Hz (603-1290Hz)
            /// 4 - 32 samples = 1376Hz (1291-2666Hz)
            /// 5 - 64 samples = 2752Hz (2667-5418Hz)
            /// 6 - 128 samples = 5504Hz (5419-10922Hz)
            /// 7 - 256 samples = 11008Hz (10923-21930Hz)
            /// Total = 510Hz

            int count = 0;
            for (int i = 0; i < FrequencyBandCount; i++)
            {
                int sampleCount = (int)Mathf.Pow(2, i) * 2;
                float average = 0;

                if (i == 7 /*FrequencyBandCount - 1*/)
                {
                    //int samples = (int)Mathf.Pow(2, FrequencyBandCount) * (22050 / SampleCount);
                    //sampleCount += SampleCount - samples;
                    //Debug.Log($"{SampleCount} - {samples} = {sampleCount}");

                    sampleCount += 2;
                }

                for (int j = 0; j < sampleCount; j++)
                {
                    average += Samples[count] * (count + 1);
                    count++;
                }

                average /= count;
                FrequencyBands[i] = average * 10;
            }
        }
    }
}
