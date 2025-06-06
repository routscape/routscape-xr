using UnityEngine;

public class RouteBehavior : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    public RouteData RouteData;
    private Mesh _bakedMesh;
    private NetworkEventDispatcher _networkEventDispatcher;
    
    void Start()
    {
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
    }
    
    public void Init(RouteData routeData)
    {
        routeData.OnRoutePointAdded += AddPoint;
        routeData.OnRoutePointModified += ModifyPoint;
        routeData.OnRouteBakeMesh += BakeMesh;
        routeData.OnDelete += DeleteSelf;
        RouteData = routeData;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("[RouteBehavior] CollisionEnter");
        _networkEventDispatcher.RPC_EraseMapObject(RouteData.ID);
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

    private void BakeMesh()
    {
        if (_bakedMesh == null)
        {
            _bakedMesh = new Mesh { name = $"{name}_SplineMesh" };
        } else
        {
            _bakedMesh.Clear();
        }
        
        BuildColliders(lineRenderer);
    }
    
    void BuildColliders(LineRenderer lr)
    {
        float radius = 0.5f;
        // clear old ones when rebaking
        foreach (var c in GetComponentsInChildren<CapsuleCollider>())
            Destroy(c);

        int count = lr.positionCount;
        for (int i = 0; i < count - 1; ++i)
        {
            Vector3 p0 = lr.GetPosition(i);
            Vector3 p1 = lr.GetPosition(i + 1);
            Vector3 mid = (p0 + p1) * 0.5f;

            float len   = Vector3.Distance(p0, p1);

            var go = new GameObject($"SegCol_{i}");
            go.layer = 8;
            go.transform.SetParent(lr.transform, false);   // keep in same local space
            go.transform.localPosition = mid;
            go.transform.up = (p1 - p0).normalized;

            var col = go.AddComponent<CapsuleCollider>();
            col.radius = radius;
            col.height = len + radius * 2f;   // Unity measures height *including* the hemispherical ends
            col.direction = 1;                // 0-x, 1-y, 2-z → we aligned Y with the segment
        }
    }

    private void DeleteSelf()
    {
        Destroy(gameObject);
    }
}
