using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Unity.Collections;
using UnityEngine;

namespace Flooding
{
    public class FloodCubeManager : MonoBehaviour
    {
        [SerializeField] private GameObject floodCubePrefab;
        [SerializeField] private AbstractMap mapManager;
        [SerializeField] private int gridSize = 64;
        [SerializeField] private Vector4 boundaries;
        [SerializeField] private float startingFloodHeight = 1f;
        [SerializeField] private float startingFloodYScale = 0.25f;
        //Distance threshold in centimeters
        [SerializeField] private float lowFloodThreshold = 0.02f;
        [SerializeField] private float highFloodThreshold = 0.05f;
        [Header("Flood Colors")]
        [SerializeField] private Color green;
        [SerializeField] private Color yellow;
        [SerializeField] private Color red;

        private FloodCube[] _floodCubes;
        private float[] _mapHeights;
        
        private float _cubeSizeX;
        private float _cubeSizeZ;

        private void Start()
        {
             _floodCubes = new FloodCube[gridSize * gridSize];
             _mapHeights = new float[gridSize * gridSize];
             GenerateCubes(); 
             mapManager.OnUpdated += Init;
        }

        private void Init()
        {
            GetMapHeights();
            SetCubeColors();
        }

        private void GenerateCubes()
        {
            _cubeSizeX = (boundaries.y - boundaries.x) / gridSize;
            _cubeSizeZ = (boundaries.w - boundaries.z) / gridSize;

            int index = 0;
            for (var x = 0; x < gridSize; x++)
            for (var z = 0; z < gridSize; z++)
            {
                var position = new Vector3(
                    boundaries.x + x * _cubeSizeX + _cubeSizeX / 2,
                    startingFloodHeight,
                    boundaries.z + z * _cubeSizeZ + _cubeSizeZ / 2
                );

                var cube = Instantiate(floodCubePrefab, position, Quaternion.identity);
                cube.transform.localScale = new Vector3(_cubeSizeX, startingFloodYScale, _cubeSizeZ);
                cube.name = $"FloodCube_{x}_{z}";
                cube.transform.SetParent(transform);
                _floodCubes[index] = cube.GetComponent<FloodCube>();
                _floodCubes[index].enabled = true;
                index++;
            }
        }

        //TODO: Use bounds instead of transform's position...
        private void SetCubeColors()
        {
            for (int i = 0; i < gridSize * gridSize; i++)
            {
                Color color = new Color();
                var distance = Mathf.Abs(_floodCubes[i].transform.position.y - _mapHeights[i]);
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
            }
        }

        private void GetMapHeights()
        {
            for (int i = 0; i < _floodCubes.Length; i++)
            {
                var latLong = mapManager.WorldToGeoPosition(_floodCubes[i].transform.position);
                _mapHeights[i] = GetHeightFromLatLong(latLong);
            }
        }

        private float GetHeightFromLatLong(Vector2d latlong)
        {
            //Find the tile given latlong
            var tileIDUnwrapped = TileCover.CoordinateToTileId(latlong, (int)mapManager.Zoom);
            UnityTile tile = mapManager.MapVisualizer.GetUnityTileFromUnwrappedTileId(tileIDUnwrapped);

            Vector2d v2d = Conversions.LatLonToMeters(latlong);
            Vector2d v2dcenter = tile.Rect.Center - new Mapbox.Utils.Vector2d(tile.Rect.Size.x / 2, tile.Rect.Size.y / 2);
            Vector2d diff = v2d - v2dcenter;

            float Dx = (float)(diff.x / tile.Rect.Size.x);
            float Dy = (float)(diff.y / tile.Rect.Size.y);

            //height in unity units
            return tile.QueryHeightData(Dx,Dy);
        }
    }
}