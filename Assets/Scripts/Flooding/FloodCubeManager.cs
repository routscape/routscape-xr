using Gestures;
using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;

namespace Flooding
{
    public class FloodCubeManager : MonoBehaviour
    {
        [SerializeField] private GameObject floodCubePrefab;
        [SerializeField] private AbstractMap mapManager;
        [SerializeField] private MapZoomHandler mapZoomHandler;
        [SerializeField] private GestureManager gestureManager;
        [SerializeField] private int gridSize = 64;

        //Distance threshold in centimeters
        [SerializeField] private float lowFloodThreshold = 0.02f;
        [SerializeField] private float highFloodThreshold = 0.05f;

        [Header("Flood Colors")] [SerializeField]
        private Color green;

        [SerializeField] private Color yellow;
        [SerializeField] private Color red;

        public float floodHeight = 3000f;
        private Vector4 _boundaries;

        private float _cubeSizeX;
        private float _cubeSizeZ;

        private FloodCube[] _floodCubes;
        private float[] _mapHeights; //map heights with respect to its own local space
        private Vector3[] _planePositions; //plane heights with respect to the map local space

        private void Start()
        {
            _floodCubes = new FloodCube[gridSize * gridSize];
            _mapHeights = new float[gridSize * gridSize];
            
            GenerateCubes();
            ReScaleHeight();
            
            mapZoomHandler.OnZoom += ReScaleHeight;
            gestureManager.OnGestureEnd += RenderCubes;
            gestureManager.OnGestureEnd += ReScaleHeight;
        }

        private void RenderCubes()
        {
            GetMapHeights();
            SetCubeColors();
        }

        private void GenerateCubes()
        {
            _cubeSizeX = (_boundaries.y - _boundaries.x) / gridSize;
            _cubeSizeZ = (_boundaries.w - _boundaries.z) / gridSize;

            var index = 0;
            for (var x = 0; x < gridSize; x++)
            for (var z = 0; z < gridSize; z++)
            {
                var position = new Vector3(
                    _boundaries.x + x * _cubeSizeX + _cubeSizeX / 2,
                    0,
                    _boundaries.z + z * _cubeSizeZ + _cubeSizeZ / 2
                );

                var cube = Instantiate(floodCubePrefab, position, Quaternion.identity);
                cube.transform.localScale = new Vector3(_cubeSizeX, 1, _cubeSizeZ);
                cube.name = $"FloodCube_{x}_{z}";
                cube.transform.SetParent(transform, true);
                //Bring back position to localspace Y=0
                cube.transform.localPosition =
                    new Vector3(cube.transform.localPosition.x, 0, cube.transform.localPosition.z);
                _floodCubes[index] = cube.GetComponent<FloodCube>();
                _floodCubes[index].enabled = true;
                index++;
            }
        }
        
        private void GetMapHeights()
        {
            for (var i = 0; i < _floodCubes.Length; i++)
            {
                var position = _floodCubes[i].transform.position;
                var latLong = mapManager.WorldToGeoPosition(new Vector3(position.x, transform.position.y, position.z));
                _mapHeights[i] = mapManager.GeoToWorldPosition(latLong).y;
            }
        }

        //Method when map zooms in
        private void ReScaleHeight()
        {
            var referenceTileRect =
                Conversions.TileBounds(TileCover.CoordinateToTileId(mapManager.CenterLatitudeLongitude,
                    mapManager.AbsoluteZoom));
            double zoomDifference = mapManager.Zoom - mapManager.AbsoluteZoom;
            double floodLevelMeters = floodHeight / 100;
            var unitsPerMeter = mapManager.Options.scalingOptions.unityTileSize / referenceTileRect.Size.x *
                                Mathd.Pow(2d, zoomDifference);
            transform.localPosition = new Vector3(transform.localPosition.x, (float)(floodLevelMeters * unitsPerMeter),
                transform.localPosition.z);
        }

        //TODO: Use bounds instead of transform's position...
        private void SetCubeColors()
        {
            for (var i = 0; i < gridSize * gridSize; i++)
            {
                var color = new Color();
                var distance = Mathf.Abs(transform.position.y - _mapHeights[i]);
                if (distance <= lowFloodThreshold)
                {
                    // Smooth gradient from green to yellow
                    var t = Mathf.InverseLerp(0, lowFloodThreshold, distance);
                    color = Color.Lerp(green, yellow, t);
                }
                else if (distance <= highFloodThreshold)
                {
                    // Smooth gradient from yellow to red
                    var t = Mathf.InverseLerp(lowFloodThreshold, highFloodThreshold, distance);
                    color = Color.Lerp(yellow, red, t);
                }
                else
                {
                    color = red;
                }

                _floodCubes[i].SetColor(color);
                _floodCubes[i].SetText(transform.position.y + ", " + _mapHeights[i]);
            }
        }

        private float GetHeightFromLatLong(Vector2d latlong)
        {
            //Find the tile given latlong
            var tileIDUnwrapped = TileCover.CoordinateToTileId(latlong, (int)mapManager.Zoom);
            var tile = mapManager.MapVisualizer.GetUnityTileFromUnwrappedTileId(tileIDUnwrapped);

            var v2d = Conversions.LatLonToMeters(latlong);
            var v2dcenter = tile.Rect.Center - new Vector2d(tile.Rect.Size.x / 2, tile.Rect.Size.y / 2);
            var diff = v2d - v2dcenter;

            var Dx = (float)(diff.x / tile.Rect.Size.x);
            var Dy = (float)(diff.y / tile.Rect.Size.y);

            //height in unity units
            return tile.QueryHeightData(Dx, Dy);
        }

        public void OnCalibrate()
        {
            DestroyCubes();
            GenerateCubes();
        }

        public void SetBoundaries(Vector4 boundaries)
        {
            _boundaries = boundaries;
        }

        public void DestroyCubes()
        {
            // Destroy all children
            foreach (Transform child in transform) Destroy(child.gameObject);
        }
    }
}