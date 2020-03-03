using UnityEngine;

namespace AudioVisualization
{
    [RequireComponent(typeof(Light))]
    public class AudioControlledLight : MonoBehaviour
    {
        [SerializeField]
        private int _band = 0;

        [SerializeField]
        private float _minIntensity = 0f;

        [SerializeField]
        private float _maxIntensity = 1f;

        private Light _light;

        private void Awake()
        {
            _light = GetComponent<Light>();
        }

        private void Update()
        {
            _light.intensity = (AudioPeer.AudioBandBuffers[_band] * (_maxIntensity - _minIntensity)) + _minIntensity;
        }
    }
}
