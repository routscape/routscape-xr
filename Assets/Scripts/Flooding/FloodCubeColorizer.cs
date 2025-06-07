using Mapbox.Unity.Map;
using UnityEngine;

namespace Flooding
{
    public class FloodCubeColorizer : MonoBehaviour
    {
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        [SerializeField] private float alpha = 0.5f;
        private Bounds _bounds;

        private float _distanceToMap;
        private float _highFloodThreshold = 0.05f;
        private float _lowFloodThreshold = 0.02f;

        private AbstractMap _map;
        private MeshFilter _mf;

        private MaterialPropertyBlock _propertyBlock;
        private Renderer _renderer;
        private Color _transparentGreen;
        private Color _transparentRed;
        private Color _transparentYellow;

        private void Start()
        {
            _transparentGreen = new Color(0, 1, 0, alpha);
            _transparentYellow = new Color(1, 1, 0, alpha);
            _transparentRed = new Color(1, 0, 0, alpha);
            _renderer = GetComponent<Renderer>();
            _mf = GetComponent<MeshFilter>();
            _bounds = _renderer.bounds;
            _propertyBlock = new MaterialPropertyBlock();
        }

        private void FixedUpdate()
        {
            if (_map == null)
            {
                Debug.LogWarning(
                    "Map is not initialized. Please call InitializeMap with a valid AbstractMap instance.");
                return;
            }

            _mf.sharedMesh.RecalculateBounds();
            _bounds = _renderer.bounds;
            var cubeTop = new Vector3(_bounds.center.x, _bounds.max.y, _bounds.center.z);

            var cubeLatLon = _map.WorldToGeoPosition(cubeTop);
            var mapPosition = _map.GeoToWorldPosition(cubeLatLon);
            _distanceToMap = Vector3.Distance(cubeTop, mapPosition);
        }

        private void LateUpdate()
        {
            if (_map == null)
            {
                Debug.LogWarning(
                    "Map is not initialized. Please call InitializeMap with a valid AbstractMap instance.");
                return;
            }

            Color color;
            if (_distanceToMap <= _lowFloodThreshold)
            {
                // Smooth gradient from green to yellow
                var t = Mathf.InverseLerp(0, _lowFloodThreshold, _distanceToMap);
                color = Color.Lerp(_transparentGreen, _transparentYellow, t);
            }
            else if (_distanceToMap <= _highFloodThreshold)
            {
                // Smooth gradient from yellow to red
                var t = Mathf.InverseLerp(_lowFloodThreshold, _highFloodThreshold, _distanceToMap);
                color = Color.Lerp(_transparentYellow, _transparentRed, t);
            }
            else
            {
                color = _transparentRed;
            }

            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorProperty, color);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public void InitializeMap(AbstractMap map)
        {
            _map = map;
        }

        public void SetFloodThresholds(float lowFlood, float highFlood)
        {
            _lowFloodThreshold = lowFlood;
            _highFloodThreshold = highFlood;
        }
    }
}