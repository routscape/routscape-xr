using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;

public class RouteDrawer : NetworkBehaviour
{
    [SerializeField] private LayerMask mapboxLayer;
    private AbstractMap _mapManager;
    private Transform _tipPoint;
    public Action<int, Vector2d> OnPencilHit;
    public float minDistanceBetweenPoints = 0.003f * 10000f;
    public float rayDistance = 0.05f;
    
    private int _currentRouteID;
    private RouteData _currentRoute;
    private Vector3 LastPoint;
    private bool _hasTipPointSet;

    [Networked]
    [OnChangedRender(nameof(AddPoint))]
    private Vector2 LatLong{ get; set; }

    public override void Spawned()
    {
        _mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
        _currentRouteID = -1;
        minDistanceBetweenPoints = 0.003f * 10000f;
    }

    public void SetTipPoint(Transform tipTransform)
    {
        _tipPoint = tipTransform;
        _hasTipPointSet = true;
        Object.RequestStateAuthority();
    }
    
    public void SetCurrentRoute(RouteData routeData)
    {
        Debug.Log("[RouteDrawer] SetCurrentRoute ID" + routeData.ID);
        _currentRouteID = routeData.ID;
        _currentRoute = routeData;
    }
    
    public void EndRoute()
    {
        _currentRouteID = -1;
        _hasTipPointSet = false;
    }
    
    void FixedUpdate()
    {
        if (_currentRouteID == -1 || !_hasTipPointSet) return;

        Vector3 hitPoint;
        Vector2d latLong;
        if (GetFingerHitPoint(out hitPoint)) // Ensure raycast hits something
        {
            var distance = Vector3.Distance(hitPoint, LastPoint);
            distance *= 10000; // floating comparison sucks
            if (distance > minDistanceBetweenPoints)
            {
                Debug.Log("[RouteDrawer] New Point Added to Route: " + _currentRouteID);
                LastPoint = hitPoint;
                latLong = _mapManager.WorldToGeoPosition(LastPoint);
                LatLong = new Vector2((float)latLong.x, (float)latLong.y);
            }
        }
    }
    
    private void AddPoint()
    {
        var latLong = new Vector2d(LatLong.x, LatLong.y);
        var worldPos = _mapManager.GeoToWorldPosition(latLong);
        _currentRoute.AddPoint(latLong, worldPos); 
    }
    
    private bool GetFingerHitPoint(out Vector3 adjustedPoint)
    {
        adjustedPoint = Vector3.zero;
        // **Raycast downward to detect the map**
        if (Physics.Raycast(_tipPoint.position, Vector3.down, out var hit, rayDistance, mapboxLayer))
        {
            adjustedPoint = hit.point + Vector3.up * 0.01f; // Slight offset
            // Debug.Log($"Finger hit detected at: {adjustedPoint}");
            return true;
        }
        return false;
    }
}