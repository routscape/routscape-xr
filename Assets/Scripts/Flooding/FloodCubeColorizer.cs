using Mapbox.Unity.Map;
using UnityEngine;

namespace Flooding
{
    public class FloodCubeColorizer : MonoBehaviour
    {
        [SerializeField] private float alpha = 0.5f;
        private Bounds _bounds;

        private float _distanceToMap;
        private AbstractMap _map;
        private Renderer _renderer;
        private Color _transparentGreen;
        private Color _transparentRed;

        private void Start()
        {
            _transparentGreen = new Color(0, 1, 0, alpha);
            _transparentRed = new Color(1, 0, 0, alpha);
            _renderer = GetComponent<Renderer>();
            _bounds = _renderer.bounds;
        }

        private void FixedUpdate()
        {
            if (_map == null)
            {
                Debug.LogWarning(
                    "Map is not initialized. Please call InitializeMap with a valid AbstractMap instance.");
                return;
            }

            var position = transform.position;
            var topYPosition = new Vector3(position.x, _bounds.max.y, position.z);
            var cubeLatLon = _map.WorldToGeoPosition(topYPosition);
            var mapPosition = _map.GeoToWorldPosition(cubeLatLon);
            _distanceToMap = Vector3.Distance(topYPosition, mapPosition);
        }

        private void LateUpdate()
        {
            if (_map == null)
            {
                Debug.LogWarning(
                    "Map is not initialized. Please call InitializeMap with a valid AbstractMap instance.");
                return;
            }

            var color = Color.Lerp(_transparentRed, _transparentGreen, _distanceToMap / 100f);

            var renderer = GetComponent<Renderer>();
            renderer.material.SetColor("_Color", color);
        }

        public void InitializeMap(AbstractMap map)
        {
            _map = map;
        }
    }
}