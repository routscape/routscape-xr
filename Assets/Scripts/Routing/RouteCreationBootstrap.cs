using UnityEngine;

public class RouteCreationBootstrap : MonoBehaviour
{
    [Header("Scene Dependencies")] 
    [SerializeField] MapObjectsManager mapObjectsManager;
    [SerializeField] RouteDrawer routeDrawer;
    [SerializeField] NetworkEventDispatcher networkEventDispatcher;

    private RouteCreationCoordinator _routeCreationCoordinator;

    void Start()
    {
        _routeCreationCoordinator = new RouteCreationCoordinator(mapObjectsManager, routeDrawer, networkEventDispatcher);
    }
}

