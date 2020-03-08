using UnityEngine;

namespace AudioVisualization
{
    public class FrequencyBand : MonoBehaviour
    {
        [SerializeField]
        private AudioPeer _audioPeer = null;

        [SerializeField]
        private int _band = 0;

        [SerializeField]
        private float _initialScale = 1f;

        [SerializeField]
        private float _scaleMultiplier = 1f;

        [SerializeField]
        private bool _useBuffer = true;

        private Material _material = null;

        private void Awake()
        {
            _material = GetComponent<MeshRenderer>().material;
        }

        private void Update()
        {
            float sample = _useBuffer ? _audioPeer.AudioBandBuffers[_band] : _audioPeer.AudioBands[_band];
            float scaleY = (sample * _scaleMultiplier) + _initialScale;

            /// Scale
            transform.localScale = new Vector3(transform.localScale.x, scaleY, transform.localScale.z);

            /// Emission
            _material.SetColor("_EmissionColor", new Color(sample, sample, sample));
        }
    }
}
