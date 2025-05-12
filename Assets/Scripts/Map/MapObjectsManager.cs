using System;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Unity.Mathematics;
using UnityEngine;

public class MapObjectsHandler: MonoBehaviour
{
    [SerializeField] private GameObject mapPinPrefab;
    
    private List<Route> _spawnedRoutes = new List<Route>();
    private List<Pin> _spawnedPins = new List<Pin>();
    private AbstractMap _mapManager;
    void Start()
    {
        _mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
        if (_mapManager == null)
        {
            Debug.LogError("[MapObjectsHandler] Missing mapbox map with tag 'mapbox map'");
        }
        
        NetworkEventDispatcher.OnPinDrop += AddPin;
    }

    private void FixedUpdate()
    {
        foreach (var pin in _spawnedPins)
        {
            var worldPosition = _mapManager.GeoToWorldPosition(pin.LatLong);
            pin.UpdateWorldPosition(worldPosition);
            pin.UpdateWorldScale(GetPinScale(_mapManager.Zoom));
        }
    }

    void AddPin(string pinName, double x, double y, int colorType)
    {
        var instantiatedPin = Instantiate(mapPinPrefab, gameObject.transform);
        var pinNewScale = GetPinScale(_mapManager.Zoom);
        instantiatedPin.transform.localScale = new Vector3(pinNewScale, pinNewScale, pinNewScale);
        instantiatedPin.transform.position = _mapManager.GeoToWorldPosition(new Vector2d(x, y));
        
        var pin = new Pin(instantiatedPin, pinName, new Vector2d(x, y), (ColorType)colorType);
        _spawnedPins.Add(pin);
    }
    
    public static float GetPinScale(float zoom)
    {
        if (zoom >= 18f) return 400f;

        if (zoom >= 17f)
        {
            float t = Mathf.InverseLerp(17f, 18f, zoom); 
            return Mathf.Lerp(300f, 400f, t);
        }

        if (zoom >= 16f)
        {
            float t = Mathf.InverseLerp(16f, 17f, zoom);
            return Mathf.Lerp(200f, 300f, t);
        }

        return 200f;
    }
}