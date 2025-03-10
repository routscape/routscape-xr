using System.Collections.Generic;
using Mapbox.Unity.Map;
using UnityEngine;

public class RouteManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<Route> _spawnedRoutes = new List<Route>();
    private AbstractMap _mapManager;
    void Start()
    {
        _mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
    }
    
    void FixedUpdate()
    {
        int count = _spawnedRoutes.Count;
        for (int i = 0; i < count; i++)
        {
            var route = _spawnedRoutes[i];
            for (int j = 0; j < route.lineRenderer.positionCount; j++)
            {
                var location = route.routePointsMap[j];
                route.lineRenderer.SetPosition(j, _mapManager.GeoToWorldPosition(location));
            }
        }
    }

    public void AddSpawnedRoute(Route spawnedRoute)
    {
        _spawnedRoutes.Add(spawnedRoute);
    }

    public void DeleteSpawnedRoute(Route route)
    {
        _spawnedRoutes.Remove(route);
    }
}
