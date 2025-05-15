using UnityEngine;

public class RouteCreationCoordinator
{
    private MapObjectsManager _mapObjectsManager;
    private RouteDrawer _routeDrawer;
    
    public RouteCreationCoordinator(MapObjectsManager mapObjectsHandler, RouteDrawer routeDrawer, NetworkEventDispatcher networkEventDispatcher)
    {
        _mapObjectsManager = mapObjectsHandler;
        _routeDrawer = routeDrawer;

        networkEventDispatcher.OnRouteBegin += CreateRoute;
        networkEventDispatcher.OnRouteEnd += EndRoute;
    }

    private void CreateRoute(string routeName, int colorType)
    {
        RouteData routeData = new RouteData(routeName, (ColorType)colorType);
        _mapObjectsManager.AddRoute(routeData);
        _routeDrawer.SetCurrentRoute(routeData.Id);
    }

    private void EndRoute()
    {
        _routeDrawer.EndRoute();
    }
}
