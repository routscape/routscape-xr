using UnityEngine;
using Utils;

public class RouteCreationCoordinator : MonoBehaviour
{
    [Header("Scene Dependencies")] 
    [SerializeField] MapObjectsManager mapObjectsManager;
    [SerializeField] RouteDrawer routeDrawer;
    [SerializeField] UIManager uiManager;
    [SerializeField] NetworkEventDispatcher networkEventDispatcher;

    void Start()
    {
        networkEventDispatcher.OnRouteBegin += CreateRoute;
        networkEventDispatcher.OnRouteEnd += EndRoute;
    }
    
    private void CreateRoute(string routeName, int objectCategory, int colorType)
    {
        RouteData routeData = new RouteData(routeName, (MapObjectCategory)objectCategory, (ColorType)colorType);
        mapObjectsManager.AddRoute(routeData);
        routeDrawer.SetCurrentRoute(routeData.ID);
        uiManager.AddRoute(routeData);
    }

    private void EndRoute()
    {
        routeDrawer.EndRoute();
    }
}

