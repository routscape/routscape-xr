using System.Collections.Generic;
using UnityEngine;

public class RouteBehavior : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    public RouteData RouteData;
    private List<CapsuleCollider> _colliders;
    private Mesh _bakedMesh;
    private NetworkEventDispatcher _networkEventDispatcher;
    private const float Radius = 0.0012543435f;
    
    void Start()
    {
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        _colliders = new List<CapsuleCollider>();
    }
    
    public void Init(RouteData routeData)
    {
        routeData.OnRoutePointAdded += AddPoint;
        routeData.OnRoutePointModified += ModifyPoint;
        routeData.OnRouteBakeMesh += BakeMesh;
        routeData.OnDelete += DeleteSelf;
        routeData.OnUpdateColliders += UpdateColliders;
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
        
        BuildColliders();
    }
    
    void BuildColliders()
    {
        foreach (var c in GetComponentsInChildren<CapsuleCollider>())
            Destroy(c.gameObject);   

        _colliders.Clear();

        int count = lineRenderer.positionCount;

        for (int i = 0; i < count - 1; ++i)
        {
            Vector3 p0 = lineRenderer.GetPosition(i);
            Vector3 p1 = lineRenderer.GetPosition(i + 1);

            Vector3 mid   = (p0 + p1) * 0.5f;
            float   len   = Vector3.Distance(p0, p1);

            var go = new GameObject($"SegCol_{i}");
            go.layer = 8;                    

            go.transform.position = mid;      
            go.transform.up       = (p1 - p0).normalized;

            var col = go.AddComponent<CapsuleCollider>();
            col.radius    = Radius;
            col.height    = len + Radius * 2f;   
            col.direction = 1;                  

            go.transform.SetParent(lineRenderer.transform, true);
            _colliders.Add(col);
        }
    }

    void UpdateColliders()
    {
        for(int i = 0; i < _colliders.Count; i++)
        {
            CapsuleCollider col = _colliders[i];
            Vector3 p0 = lineRenderer.GetPosition(i);
            Vector3 p1 = lineRenderer.GetPosition(i + 1);
            Vector3 mid = (p0 + p1) * 0.5f;
            float len   = Vector3.Distance(p0, p1);
            col.gameObject.transform.position= mid;
            col.height = len + Radius * 2f;
        }
    }

    private void DeleteSelf()
    {
        Destroy(gameObject);
    }
}
