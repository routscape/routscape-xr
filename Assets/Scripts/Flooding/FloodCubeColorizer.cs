using Mapbox.Unity.Map;
using UnityEngine;

namespace Flooding
{
    public class FloodCubeColorizer : MonoBehaviour
    {
        private float _distanceToMap;
        private AbstractMap _map;

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

            var color = Color.Lerp(Color.red, Color.green, _distanceToMap / 100f);
            GetComponent<Renderer>().material.color = color;
        }

        public void InitializeMap(AbstractMap map)
        {
            _map = map;
        }
    }
}