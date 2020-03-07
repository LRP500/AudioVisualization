using UnityEngine;

namespace AudioVisualization
{
    public class AudioVisualizer : MonoBehaviour
    {
        [SerializeField]
        private GameObject _sampleViewPrefab = null;

        [SerializeField]
        private float _maxScale = 1f;

        private GameObject[] _sampleViews = null;

        private void Awake()
        {
            if (_sampleViewPrefab)
            {
                InstantiateSampleViews();
            }
        }

        private void Update()
        {
            if (_sampleViews != null)
            {
                for (int i = 0; i < AudioPeer.SampleCount; i++)
                {
                    Vector3 scale = new Vector3(10, ((AudioPeer.LeftSamples[i] + AudioPeer.RightSamples[i]) * _maxScale) + 2, 10);
                    _sampleViews[i].transform.localScale = scale;
                }
            }
        }

        private void InstantiateSampleViews()
        {
            _sampleViews = new GameObject[AudioPeer.SampleCount];

            for (int i = 0; i < _sampleViews.Length; i++)
            {
                GameObject instance = Instantiate(_sampleViewPrefab);
                instance.transform.position = transform.position;
                instance.transform.SetParent(transform);
                transform.eulerAngles = new Vector3(0, -(360f / _sampleViews.Length) * i, 0);
                instance.transform.position = Vector3.forward * 100;
                instance.name = $"SampleView [{i}]";
                _sampleViews[i] = instance;
            }
        }
    }
}
