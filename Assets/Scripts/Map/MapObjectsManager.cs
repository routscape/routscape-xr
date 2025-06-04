using System;
using System.Collections.Generic;
using Gestures;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Unity.Mathematics;
using UnityEngine;
using Utils;

public class MapObjectsManager: MonoBehaviour
{
    [SerializeField] private GameObject mapPinBehavior;
    [SerializeField] private GameObject mapRouteBehavior;
    [SerializeField] private AbstractMap mapManager;
    [SerializeField] private RouteDrawer routeDrawer;
    
    private Dictionary<MapObjectCategory, GameObject> _mapLayers = new Dictionary<MapObjectCategory, GameObject>();
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

        _networkEventDispatcher.OnJumpToMapObject += JumpTo;
        routeDrawer.OnPencilHit += AddPointToRoute;
        LayerStateManager.I.LayerStateChanged += OnLayerStateChanged;
        
        InitializeLayers();
    }

    //TODO: Optimize via maponselect and mapdeselect events
    void Update()
    {
        foreach (var pin in _spawnedPins)
        {
            var worldPosition = mapManager.GeoToWorldPosition(pin.LatLong);
            pin.UpdateWorldPosition(worldPosition);
            pin.UpdateWorldScale(GetPinScale(mapManager.Zoom));
        }

        foreach (var route in _spawnedRoutes)
        {
            var points = route.routePointsLatLong;
            for (int i = 0; i < points.Count; i++)
            {
                var latLong = route.routePointsLatLong[i];
                var worldPosition = mapManager.GeoToWorldPosition(latLong);
                route.SetVertexPosition(i, worldPosition);
            }
        }
    }

    void InitializeLayers()
    {
        foreach (var mapObjectType in MapObjectCatalog.I.mapObjectTypes)
        {
            var go = new GameObject()
            {
                name = mapObjectType.displayName,
            };
            go.transform.SetParent(transform, false);
            _mapLayers[mapObjectType.objectCategory] = go;
        } 
    }

    void OnLayerStateChanged()
    {
        foreach (var layerState in LayerStateManager.I.LayerStates)
        {
            Debug.Log("[MapObjectsManager] Layer State: " + layerState.State);
            _mapLayers[layerState.ObjectCategory].SetActive(layerState.State);
        }
    }
    
    private void JumpTo(int objectID)
    {
        Vector2d latLong = Vector2d.zero;
        if (_spawnedRoutes.Exists(r=> r.ID == objectID))
        {
            latLong = _spawnedRoutes.Find(r => r.ID == objectID).GetLocation();
        } else if (_spawnedPins.Exists(p => p.ID == objectID))
        {
            latLong = _spawnedPins.Find(p => p.ID == objectID).LatLong;
        }
        
        mapManager.UpdateMap(latLong);
    }

    public void AddRoute(RouteData routeData)
    {
        var parentLayer = _mapLayers[routeData.ObjectCategory];
        var instantiatedRoute = Instantiate(mapRouteBehavior, parentLayer.transform);
        var routeBehavior = instantiatedRoute.GetComponent<RouteBehavior>();
        _spawnedRoutes.Add(routeData);
        routeBehavior.Init(routeData);
    }
    
    void AddPointToRoute(int routeID, Vector3 point)
    {
        var routeData = _spawnedRoutes.Find(r => r.ID == routeID);
        var latLong = mapManager.WorldToGeoPosition(point);
        routeData.AddPoint(latLong, point);
    }
    
    public void AddPin(PinData pinData)
    {
        var latLong = mapManager.WorldToGeoPosition(pinData.WorldPosition);
        var scale= GetPinScale(mapManager.Zoom);
        _spawnedPins.Add(pinData);

        var visualPrefab = MapObjectCatalog.I.mapObjectTypes.Find(mapObjectType => mapObjectType.objectCategory == pinData.ObjectCategory).visualPrefab;
        var parentLayer = _mapLayers[pinData.ObjectCategory];
        var instantiatedBehavior = Instantiate(mapPinBehavior, parentLayer.transform);
        var instantiatedVisual = Instantiate(visualPrefab, instantiatedBehavior.transform);
        instantiatedVisual.GetComponent<Animator>().enabled = true;
        var pinBehaviorComponent = instantiatedBehavior.GetComponent<PinBehavior>();
        pinBehaviorComponent.Init(pinData);
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