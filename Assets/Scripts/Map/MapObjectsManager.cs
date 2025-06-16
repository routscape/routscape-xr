using System;
using System.Collections.Generic;
using System.Linq;
using Gestures;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Utils;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Utils;

public class MapObjectsManager : MonoBehaviour
{
    [SerializeField] private GameObject mapPinBehavior;
    [SerializeField] private GameObject mapRouteBehavior;
    [SerializeField] private GameObject mapObjectText;
    [SerializeField] private AbstractMap mapManager;
    [SerializeField] private MapZoomHandler mapZoomHandler;

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

        _networkEventDispatcher =
            GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        if (_networkEventDispatcher == null)
        {
            Debug.Log("[PinDropper] No network event dispatcher found!");
            throw new Exception("[PinDropper] No network event dispatcher found!");
        }

        _networkEventDispatcher.OnJumpToMapObject += JumpTo;
        _networkEventDispatcher.OnRepositionPin += RepositionPin;
        _networkEventDispatcher.OnGestureEnd += UpdateRouteColliders;
        mapManager.OnUpdated += OnMapUpdated;
        LayerStateManager.I.LayerStateChanged += OnLayerStateChanged;

        InitializeLayers();
    }

    void OnMapUpdated()
    {
        foreach (var pin in _spawnedPins)
        {
            var worldPosition = mapManager.GeoToWorldPosition(pin.LatLong);
            pin.UpdateWorldPosition(worldPosition);
            pin.UpdateWorldScale(GetPinScale(mapManager.Zoom));
        }

        foreach (var route in _spawnedRoutes)
        {
            var latLongs = route.RoutePointsLatLong;
            for (int i = 0; i < latLongs.Count; i++)
            {
                var newWorldPosition = mapManager.GeoToWorldPosition(latLongs[i]);
                route.SetVertexPosition(i, newWorldPosition);
            }
        }
    }

    void UpdateRouteColliders()
    {
        foreach (var route in _spawnedRoutes)
        {
            route.UpdateColliders();
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
        if (_spawnedRoutes.Exists(r => r.ID == objectID))
        {
            latLong = _spawnedRoutes.Find(r => r.ID == objectID).GetLocation();
        }
        else if (_spawnedPins.Exists(p => p.ID == objectID))
        {
            latLong = _spawnedPins.Find(p => p.ID == objectID).LatLong;
        }

        mapManager.UpdateMap(latLong);
    }

    public void AddRoute(RouteData routeData)
    {
        var parentLayer = _mapLayers[routeData.ObjectCategory];
        var instantiatedRoute = Instantiate(mapRouteBehavior, parentLayer.transform);
        routeData.SetParentTransform(instantiatedRoute.transform);
        var latLong = mapManager.WorldToGeoPosition(routeData.ParentTransform.TransformPoint(routeData.ParentTransform.position)); //turn the route gameobject's localposition to worldspace
        routeData.SetParentLatLong(latLong);
        var routeBehavior = instantiatedRoute.GetComponent<RouteBehavior>();
        _spawnedRoutes.Add(routeData);
        routeBehavior.Init(routeData);
    }

    public void AddPin(PinData pinData)
    {
        var worldPos = mapManager.GeoToWorldPosition(pinData.LatLong);
        var scale = GetPinScale(mapManager.Zoom);
        _spawnedPins.Add(pinData);

        var visualPrefab = MapObjectCatalog.I.GetTypeInfo(pinData.ObjectCategory).visualPrefab;
        var parentLayer = _mapLayers[pinData.ObjectCategory];
        var instantiatedBehavior = Instantiate(mapPinBehavior, parentLayer.transform);
        var instantiatedVisual = Instantiate(visualPrefab, instantiatedBehavior.transform);
        var instantiatedText = Instantiate(mapObjectText, instantiatedBehavior.transform, false);
        
        var textComponent = instantiatedText.GetComponent<TMP_Text>();
        textComponent.text = pinData.Name; 
        var visualMeshRenderers = instantiatedVisual.GetComponentsInChildren<MeshRenderer>();
        instantiatedVisual.GetComponent<Animator>().enabled = true;
        var pinBehaviorComponent = instantiatedBehavior.GetComponent<PinBehavior>();
        
        pinBehaviorComponent.meshRenderers = visualMeshRenderers;
        pinData.UpdateWorldPosition(worldPos);
        pinData.UpdateWorldScale(scale);
        pinBehaviorComponent.Init(pinData);
        pinBehaviorComponent.SetTextComponent(instantiatedText);
    }

    private void RepositionPin(int objectID, double latitude, double longitude)
    {
        if (!_spawnedPins.Exists(p => p.ID == objectID))
        {
            return;
        }
        
        var pinData = _spawnedPins.Find(p => p.ID == objectID);
        var newLatLong = new Vector2d(latitude, longitude);
        var newWorldPos = mapManager.GeoToWorldPosition(newLatLong);
        pinData.ChangeLatLong(newLatLong);
        pinData.UpdateWorldPosition(newWorldPos);
    }

    public void DeleteMapObject(int objectID)
    {
        if (_spawnedRoutes.Exists(r => r.ID == objectID))
        {
            var mapObject = _spawnedRoutes.Find(r => r.ID == objectID);
            mapObject.DeleteSelf();
            _spawnedRoutes.Remove(mapObject);
        } else if (_spawnedPins.Exists(p => p.ID == objectID))
        {
            var mapObject = _spawnedPins.Find(r => r.ID == objectID);
            mapObject.DeleteSelf();
            _spawnedPins.Remove(mapObject);
        } else
        {
            Debug.LogWarning("[MapObjectsHandler] You tried to delete a non existent object with ID " + objectID);
        }
    }
    
    private static float GetPinScale(float zoom)
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