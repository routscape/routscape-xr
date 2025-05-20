using System;
using System.Collections.Generic;
using Gestures;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Unity.Mathematics;
using UnityEngine;

public class MapObjectsManager: MonoBehaviour
{
    [SerializeField] private GameObject mapPinPrefab;
    [SerializeField] private GameObject mapRoutePrefab;
    [SerializeField] private AbstractMap mapManager;
    [SerializeField] private RouteDrawer routeDrawer;
    
    private List<RouteData> _spawnedRoutes = new List<RouteData>();
    private List<PinData> _spawnedPins = new List<PinData>();
    private NetworkEventDispatcher _networkEventDispatcher;
    
    void Start()
    {
        if (mapManager == null)
        {
            Debug.LogError("[MapObjectsHandler] Missing mapbox map in inspector!");
        }
        if (routeDrawer == null)
        {
            Debug.LogError("[MapObjectsHandler] Missing routeDrawer in inspector!");
        }
        
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        if (_networkEventDispatcher== null)
        {
            Debug.Log("[PinDropper] No network event dispatcher found!");
            throw new Exception("[PinDropper] No network event dispatcher found!");
        }
        
        routeDrawer.OnPencilHit += AddPointToRoute;
        mapManager.OnUpdated += UpdateObjects;
    }

    private void UpdateObjects()
    {
        foreach (var pin in _spawnedPins)
        {
            var worldPosition = mapManager.GeoToWorldPosition(pin.LatLong);
            pin.UpdateWorldPosition(worldPosition);
            pin.UpdateWorldScale(GetPinScale(mapManager.Zoom));
        }
    }

    public void AddRoute(RouteData routeData)
    {
        _spawnedRoutes.Add(routeData);
        var instantiatedRoute = Instantiate(mapRoutePrefab, gameObject.transform);
        var routeBehavior = instantiatedRoute.GetComponent<RouteBehavior>();
        routeBehavior.Init(routeData);
    }
    void AddPointToRoute(int routeID, Vector3 point)
    {
        var routeData = _spawnedRoutes.Find(route => route.ID == routeID);
        var latLong = mapManager.WorldToGeoPosition(point);
        routeData.AddPoint(latLong, point);
    }
    public void AddPin(PinData pinData)
    {
        var latLong = mapManager.WorldToGeoPosition(pinData.WorldPosition);
        var scale= GetPinScale(mapManager.Zoom);
        _spawnedPins.Add(pinData);
        var instantiatedPin = Instantiate(mapPinPrefab, gameObject.transform);
        var pinBehavior = instantiatedPin.GetComponent<PinBehavior>(); 
        pinBehavior.Init(pinData);
        pinData.ChangeLatLong(latLong);
        pinData.UpdateWorldScale(scale);
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