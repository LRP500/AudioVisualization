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

        private void Update()
        {
            float height = (AudioPeer.FrequencyBands[_band] * _scaleMultiplier) + _initialScale;
            transform.localScale = new Vector3(transform.localScale.x, height, transform.localScale.z);    
        }
    }
}
