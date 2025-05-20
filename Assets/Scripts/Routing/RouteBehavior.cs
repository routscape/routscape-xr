using UnityEngine;

public class RouteBehavior : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    public void Init(RouteData routeData)
    {
        routeData.OnRoutePointAdded += AddPoint;
        routeData.OnRoutePointModified += ModifyPoint;
    }
    
    private void AddPoint(Vector3 worldPoint)
    {
        lineRenderer.positionCount++;
        Debug.Log("[RouteBehavior] point count: " + lineRenderer.positionCount + " | point added: " + worldPoint);
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, worldPoint);
    }

    private void ModifyPoint(int index, Vector3 worldPoint)
    {
       lineRenderer.SetPosition(index, worldPoint); 
    }
}
