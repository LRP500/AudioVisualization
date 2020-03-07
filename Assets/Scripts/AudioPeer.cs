using System.Collections.Generic;
using UnityEngine;

namespace AudioVisualization
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPeer : MonoBehaviour
    {
        public enum Channel
        {
            Stereo,
            Left,
            Right
        }

        /// <summary>
        /// Average value added to frequency highests at start
        /// to smooth frequency bands interval gap.
        /// </summary>
        [SerializeField]
        private float _audioProfile = 0f;

        [SerializeField]
        private Channel _channel = Channel.Stereo;

        public static readonly int SampleCount = 512;
        public static readonly int FrequencyBandCount = 8;

        public static float[] LeftSamples { get; private set; } = null;
        public static float[] RightSamples { get; private set; } = null;

        public static float[] FrequencyBands { get; private set; } = null;
        public static float[] FrequencyBandBuffers { get; private set; } = null;

        public static float[] AudioBands { get; private set; } = null;
        public static float[] AudioBandBuffers { get; private set; } = null;

        public static float Amplitude { get; private set; } = 0f;
        public static float AmplitudeBuffer { get; private set; } = 0f;

        private AudioSource _audioSource = null;

        private float[] _bufferDecrease = null;
        private float[] _frequencyBandHighests = null;
        private float _amplitudeHighest = 0f;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            _bufferDecrease = new float[FrequencyBandCount];
            _frequencyBandHighests = new float[FrequencyBandCount];

            LeftSamples = new float[SampleCount];
            RightSamples = new float[SampleCount];

            FrequencyBands = new float[FrequencyBandCount];
            FrequencyBandBuffers = new float[FrequencyBandCount];

            AudioBands = new float[FrequencyBandCount];
            AudioBandBuffers = new float[FrequencyBandCount];
        }

        private void Start()
        {
            AudioProfile(_audioProfile);
        }

        private void Update()
        {
            GetSpectrumAudioSource();
            CreateFrequencyBands();
            FrequencyBandBuffer();
            CreateAudioBands();
            CalculateAmplitude();
        }

        private void CreateAudioBands()
        {
            for (int i = 0; i < FrequencyBandCount; i++)
            {
                if (FrequencyBands[i] > _frequencyBandHighests[i])
                {
                    _frequencyBandHighests[i] = FrequencyBands[i];
                }

                AudioBands[i] = FrequencyBands[i] / _frequencyBandHighests[i];
                AudioBandBuffers[i] = (FrequencyBandBuffers[i] / _frequencyBandHighests[i]);
            }
        }

        private void CalculateAmplitude()
        {
            float currentAmplitude = 0f;
            float currentAmplitudeBuffer = 0f;

            for (int i = 0; i < FrequencyBandCount; i++)
            {
                currentAmplitude += AudioBands[i];
                currentAmplitudeBuffer += AudioBandBuffers[i];
            }

            _amplitudeHighest = Mathf.Max(_amplitudeHighest, currentAmplitude);
            Amplitude = currentAmplitude / _amplitudeHighest;
            AmplitudeBuffer = currentAmplitudeBuffer / _amplitudeHighest;
        }

        /// <summary>
        /// Smooth frequency bands interval gap on first few seconds
        /// of the simulation by initializing their highests to an average value.
        /// </summary>
        /// <param name="value"></param>
        private void AudioProfile(float value)
        {
            for (int i = 0; i < FrequencyBandCount; i++)
            {
                _frequencyBandHighests[i] = value;
            }
        }

        private void FrequencyBandBuffer()
        {
            for (int i = 0; i < FrequencyBandCount; i++)
            {
                if (FrequencyBands[i] > FrequencyBandBuffers[i])
                {
                    FrequencyBandBuffers[i] = FrequencyBands[i];
                    _bufferDecrease[i] = 0.005f;
                }

                if (FrequencyBands[i] < FrequencyBandBuffers[i])
                {
                    FrequencyBandBuffers[i] -= _bufferDecrease[i];
                    _bufferDecrease[i] *= 1.2f;
                }
            }
        }

        private void GetSpectrumAudioSource()
        {
            _audioSource.GetSpectrumData(LeftSamples, 0, FFTWindow.Blackman);
            _audioSource.GetSpectrumData(RightSamples, 1, FFTWindow.Blackman);
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
                    /// Stereo
                    if (_channel == Channel.Stereo)
                    {
                        average += (LeftSamples[count] + RightSamples[count]) * (count + 1);
                    }
                    /// Left 
                    else if (_channel == Channel.Left)
                    {
                        average += LeftSamples[count] * (count + 1);
                    }
                    /// Right
                    else if (_channel == Channel.Right)
                    {
                        average += RightSamples[count] * (count + 1);
                    }

                    count++;
                }

                average /= count;
                FrequencyBands[i] = average * 10;
            }
        }
    }
}
