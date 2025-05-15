using UnityEngine;

public class RouteBehavior : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    public void Init(RouteData routeData)
    {
        routeData.OnRoutePointAdded += OnRoutePointAdded;
    }
    private void OnRoutePointAdded(Vector3 worldPoint)
    {
        lineRenderer.positionCount++;
        Debug.Log("[RouteBehavior] point count: " + lineRenderer.positionCount + " | point added: " + worldPoint);
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, worldPoint);
    }
}
