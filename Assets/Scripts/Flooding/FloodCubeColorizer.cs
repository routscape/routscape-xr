using Mapbox.Unity.Map;
using UnityEngine;

namespace Flooding
{
    public class FloodCubeColorizer : MonoBehaviour
    {
        [SerializeField] private float alpha = 0.5f;

        private float _distanceToMap;
        private AbstractMap _map;
        private Color _transparentGreen;
        private Color _transparentRed;

        private void Start()
        {
            _transparentGreen = new Color(0, 1, 0, alpha);
            _transparentRed = new Color(1, 0, 0, alpha);
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
            var cubeLatLon = _map.WorldToGeoPosition(position);
            var mapPosition = _map.GeoToWorldPosition(cubeLatLon);
            _distanceToMap = Vector3.Distance(position, mapPosition);
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
            if (renderer != null)
                renderer.material.SetColor("_Color", color);
            else
                Debug.LogWarning($"Renderer not found on {gameObject.name}. Ensure it has a Renderer component.");
        }

        public void InitializeMap(AbstractMap map)
        {
            _map = map;
        }
    }
}