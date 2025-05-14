using System;
using System.Collections.Generic;
using Gestures;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Unity.Mathematics;
using UnityEngine;

public class MapObjectsHandler: MonoBehaviour
{
    [SerializeField] private GameObject mapPinPrefab;
    //Hacky Solution for Updating Pin Zoom
    [SerializeField] private MapZoomHandler mapZoomHandler;
    //Hacky Solution for Updating Object Locations 
    [SerializeField] private MapMovementHandler mapMovementHandler;
    
    private List<Route> _spawnedRoutes = new List<Route>();
    private List<PinData> _spawnedPins = new List<PinData>();
    private AbstractMap _mapManager;
    void Start()
    {
        _mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
        if (_mapManager == null)
        {
            Debug.LogError("[MapObjectsHandler] Missing mapbox map with tag 'mapbox map'");
        }
        
        NetworkEventDispatcher.OnPinDrop += AddPin;
        //Should be the network event dispatcher, but not a necessary refactor as of now
        mapZoomHandler.OnMapZoom += UpdateObjects;
        mapMovementHandler.OnMapMove += UpdateObjects;
    }

    private void UpdateObjects()
    {
        foreach (var pin in _spawnedPins)
        {
            var worldPosition = _mapManager.GeoToWorldPosition(pin.LatLong);
            pin.UpdateWorldPosition(worldPosition);
            pin.UpdateWorldScale(GetPinScale(_mapManager.Zoom));
        }
    }

    void AddPin(string pinName, Vector3 hitInfo, int colorType)
    {
        var latLong = _mapManager.WorldToGeoPosition(hitInfo);
        var x = latLong.x;
        var y = latLong.y;
        var scale= GetPinScale(_mapManager.Zoom);
        var worldPosition = _mapManager.GeoToWorldPosition(new Vector2d(x, y));
        
        var instantiatedPin = Instantiate(mapPinPrefab, gameObject.transform);
        var pinBehavior = instantiatedPin.GetComponent<PinBehavior>(); 
        var pinData = new PinData(pinName, new Vector2d(x, y), worldPosition, scale, (ColorType)colorType);
        pinBehavior.Init(pinData);
        pinBehavior.PinData = pinData;
        _spawnedPins.Add(pinData);
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