using UnityEngine;

namespace AudioVisualization
{
    public class FrequencyBand : MonoBehaviour
    {
        [SerializeField]
        private int _band = 0;

        [SerializeField]
        private float _initialScale = 1f;

        [SerializeField]
        private float _scaleMultiplier = 1f;

        [SerializeField]
        private bool _useBuffer = true;

        private void Update()
        {
            float sample = _useBuffer ? AudioPeer.BandBuffers[_band] : AudioPeer.FrequencyBands[_band];
            float scaleY = (sample * _scaleMultiplier) + _initialScale;
            transform.localScale = new Vector3(transform.localScale.x, scaleY, transform.localScale.z);    
        }
    }
}
