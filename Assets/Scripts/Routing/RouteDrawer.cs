using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using UnityEngine;

public class RouteDrawer : NetworkBehaviour
{
    [SerializeField] private LayerMask mapboxLayer;
    private Transform _tipPoint;
    public Action<int, Vector3> OnPencilHit;
    public float minDistanceBetweenPoints = 0.003f * 10000f;
    public float rayDistance = 0.05f;
    
    private int _currentRouteID;
    
    [Networked]
    [OnChangedRender(nameof(AddPoint))]
    private Vector3 LastPoint { get; set; }
    
    public override void Spawned()
    {
        _currentRouteID = -1;
        minDistanceBetweenPoints = 0.003f * 10000f;
    }
    
    public void SetCurrentRoute(int routeID)
    {
        Debug.Log("[RouteDrawer] SetCurrentRoute ID" + routeID);
        _currentRouteID = routeID;
        _tipPoint = GameObject.FindGameObjectWithTag("pencil tip point").transform;
    }
    
    public void EndRoute()
    {
        _currentRouteID = -1;
    }
    
    private void FixedUpdate()
    {
        if (_currentRouteID == -1 || _tipPoint == null) return;

        Vector3 hitPoint;
        if (GetFingerHitPoint(out hitPoint)) // Ensure raycast hits something
        {
            var distance = Vector3.Distance(hitPoint, LastPoint);
            distance *= 10000; // floating comparison sucks
            if (distance > minDistanceBetweenPoints)
            {
                LastPoint = hitPoint;
            }
        }
    }
    
    private void AddPoint()
    {
        OnPencilHit?.Invoke(_currentRouteID, LastPoint);
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