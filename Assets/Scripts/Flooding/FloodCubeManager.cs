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

        //Distance threshold in centimeters, for editor purposes
        [SerializeField] private double lowFloodThresholdCentimeters = 0.02f;
        [SerializeField] private double highFloodThresholdCentimeters = 0.05f;
        
        [Header("Flood Colors")] 
        [SerializeField] private Color green;
        [SerializeField] private Color yellow;
        [SerializeField] private Color red;

        //The reference flood level scaled down
        private double _constantLowFloodThreshold;
        private double _constantHighFloodThreshold;
        
        //The changing flood level
        private double _scaledLowFloodThreshold;
        private double _scaledHighFloodThreshold;
        
        public float floodHeight = 3000f;
        private Vector4 _boundaries = new(-0.5f, 0.5f, 0f, 1f);

        private float _cubeSizeX;
        private float _cubeSizeZ;

        private FloodCube[] _floodCubes;
        private float[] _mapHeights; //map heights with respect to its own local space
        private Vector3[] _planePositions; //plane heights with respect to the map local space

        private bool _isCalibrating = true;
        private float _lastFrame;

        private void Start()
        {
            _floodCubes = new FloodCube[gridSize * gridSize];
            _mapHeights = new float[gridSize * gridSize];
            
            mapZoomHandler.OnZoom += ReScaleHeight;
            gestureManager.OnGestureEnd += ReScaleHeight;
            gestureManager.OnGestureEnd += ReScaleFloodLevelThreshold;
            gestureManager.OnGestureEnd += RenderCubes;
        }

        private void InitializeFloodThresholdScales()
        {
            //Convert to meters
            _constantLowFloodThreshold = lowFloodThresholdCentimeters / 100f;
            _constantHighFloodThreshold = highFloodThresholdCentimeters / 100f;
            
            //Scale down to accomodate map scale
            _constantLowFloodThreshold *= 0.002508687f;
            _constantHighFloodThreshold *= 0.002508687f;

            //Divide by precise worldrelativescale to account for different zoom levels
            double unitsPerMeter = GetUnitsPerMeterScaled();
            _constantLowFloodThreshold /= unitsPerMeter;
            _constantHighFloodThreshold /= unitsPerMeter;
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
            double floodLevelMeters = floodHeight / 100;
            double unitsPerMeter = GetUnitsPerMeterScaled();
            transform.localPosition = new Vector3(transform.localPosition.x, (float)(floodLevelMeters * unitsPerMeter),
                transform.localPosition.z);
        }

        private void ReScaleFloodLevelThreshold()
        {
            double unitsPerMeter = GetUnitsPerMeterScaled();
            _scaledLowFloodThreshold = _constantLowFloodThreshold * unitsPerMeter;
            _scaledHighFloodThreshold = _constantHighFloodThreshold * unitsPerMeter; 
        }

        private double GetUnitsPerMeterScaled()
        {
            var referenceTileRect =
                Conversions.TileBounds(TileCover.CoordinateToTileId(mapManager.CenterLatitudeLongitude,
                    mapManager.AbsoluteZoom));
            double zoomDifference = mapManager.Zoom - mapManager.AbsoluteZoom;
            var unitsPerMeter = mapManager.Options.scalingOptions.unityTileSize / referenceTileRect.Size.x * 
                                Mathd.Pow(2d, zoomDifference);
            return unitsPerMeter;
        }

        private void SetCubeColors()
        {
            for (var i = 0; i < gridSize * gridSize; i++)
            {
                var color = new Color();
                var distance = Mathf.Abs(transform.position.y - _mapHeights[i]);
                if (distance <= _scaledLowFloodThreshold)
                {
                    // Smooth gradient from green to yellow
                    var t = Mathd.InverseLerp(0, _scaledLowFloodThreshold, distance);
                    color = Color.Lerp(green, yellow, (float)t);
                }
                else if (distance <= _scaledHighFloodThreshold)
                {
                    // Smooth gradient from yellow to red
                    var t = Mathd.InverseLerp(_scaledLowFloodThreshold, _scaledHighFloodThreshold, distance);
                    color = Color.Lerp(yellow, red, (float)t);
                }
                else
                {
                    color = red;
                }

                _floodCubes[i].SetColor(color);
            }
        }

        public void SetFloodHeight(float value)
        {
            floodHeight = value;
            ReScaleHeight();
            SetCubeColors();
        }

        public void OnCalibrate()
        {
            if (HasInputFiredTwice())
            {
                return;
            }
            
            if (_isCalibrating)
            {
                GenerateCubes();
                InitializeFloodThresholdScales();
                ReScaleHeight();
                ReScaleFloodLevelThreshold();
                RenderCubes();   
            }
            else
            { 
                DestroyCubes();
            }

            _isCalibrating = !_isCalibrating;
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
        
        private bool HasInputFiredTwice()
        {
            //Hacky solution because why do poke events fire twice?!
            if (_lastFrame == Time.frameCount)
            {
                return true;
            }
            _lastFrame = Time.frameCount;
            return false;
        }
    }
}