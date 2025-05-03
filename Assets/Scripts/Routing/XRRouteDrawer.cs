using System.Collections.Generic;
using System.Linq;
using Fusion;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using UnityEngine;

public class XRRouteDrawer : NetworkBehaviour
{
    public int numTotalRoutes;

    [SerializeField] private LayerMask mapboxLayer;
    [SerializeField] private UserInterfaceManagerScript userInterfaceManagerScript;
    [SerializeField] private OVRHand rightHand;
    [SerializeField] private OVRSkeleton.BoneId selectedFinger = OVRSkeleton.BoneId.Hand_IndexTip;
    [SerializeField] private RouteManager routeManager;
	[SerializeField] private GameObject roadsMesh;
    
    public float minDistanceBetweenPoints = 0.003f * 10000f; // Minimum distance to register a new point
    public float rayDistance = 0.05f;
    private readonly ColorType initialColor = ColorType.Blue;

    private readonly List<Route> routeList = new();
    private bool _isSpawned;

    private AbstractMap _mapManager;
    private Route currentRoute;

    [Networked]
    [OnChangedRender(nameof(AddPoint))]
    private Vector3 PointToAdd { get; set; } = Vector3.negativeInfinity;

    private void Start()
    {
        _mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
        minDistanceBetweenPoints = 0.003f;
    }

    private void FixedUpdate()
    {
        if (!_isSpawned || !Object.HasStateAuthority) return;
        if (userInterfaceManagerScript.mode != 1) return;
    
        if (GetFingerHitPoint(out Vector3 hitPoint))
        {
            hitPoint += Vector3.up * 0.01f; // Slightly above the mesh
    
            if (Vector3.Distance(hitPoint, PointToAdd) > minDistanceBetweenPoints)
            {
				Debug.Log($"Setting new PointToAdd from {PointToAdd} to {hitPoint}");
                PointToAdd = hitPoint;
            }
        }
    }

    public override void Spawned()
    {
        _isSpawned = true;
    }

    public Route CreateNewLine()
    {
        var name = "Route - " + numTotalRoutes;
        var newLineObj = new GameObject(name);
        newLineObj.transform.parent = transform;
        var newLineRenderer = newLineObj.AddComponent<LineRenderer>();
        newLineRenderer.positionCount = 0;
        newLineRenderer.startWidth = 0.008f;
        newLineRenderer.endWidth = 0.008f;
        newLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        newLineRenderer.startColor = ColorHexCodes.GetColor(initialColor);
        newLineRenderer.endColor = ColorHexCodes.GetColor(initialColor);

        var newRoute = new Route(name, name, newLineRenderer, initialColor);
        newRoute.prefab = newLineObj;
        routeList.Add(newRoute);
        SetCurrentRoute(name);
        Debug.Log("CreateNewLine");
        numTotalRoutes++;
        return newRoute;
    }

    private void AddPoint()
    {
        if (!_isSpawned) return;
        if (userInterfaceManagerScript.mode != 1) return; // Safety check

        currentRoute.AddPoint(PointToAdd, _mapManager);
    }

    private bool GetFingerHitPoint(out Vector3 adjustedPoint)
    {
        adjustedPoint = Vector3.zero;

        if (rightHand == null)
        {
            Debug.LogWarning("Right hand not assigned!");
            return false;
        }

        var skeleton = rightHand.GetComponent<OVRSkeleton>();
        if (skeleton == null || !skeleton.IsDataValid || !skeleton.IsDataHighConfidence || skeleton.Bones == null ||
            skeleton.Bones.Count == 0)
        {
            Debug.LogWarning("Skeleton data is not valid or not initialized!");
            return false;
        }

        // **Find the fingertip bone using the selectedFinger value**
        var fingertip = skeleton.Bones.FirstOrDefault(b => b.Id == selectedFinger);

        if (fingertip == null)
        {
            Debug.LogWarning($"Selected fingertip {selectedFinger} not found!");
            return false;
        }

        // Use fingertip position
        var fingerPosition = fingertip.Transform.position;

        Debug.DrawRay(fingerPosition, Vector3.up * 0.05f, Color.red, 0.1f);

        // **Raycast downward to detect the map**
        if (Physics.Raycast(fingerPosition, Vector3.down, out var hit, rayDistance, mapboxLayer))
        {
            var hitObject = hit.collider.gameObject;
            Debug.Log($"Ray hit: {hitObject.name}, tag: {hitObject.tag}, layer: {LayerMask.LayerToName(hitObject.layer)}");
    
            // All road object names start with "Untitled - "
            if (hitObject.name.StartsWith("Untitled - "))
            {
                adjustedPoint = hit.point + Vector3.up * 0.01f; // Slight offset
                Debug.Log($"Finger hit detected on: {hitObject.name}");
                return true;
            }
            else
            {
                // If a map tile is hit, then the road hit is in the hitObject's children
                foreach (Transform child in hitObject.transform)
                {
                    if (child.name.StartsWith("Untitled - "))
                    {
                        adjustedPoint = child.position + Vector3.up * 0.01f;
                        Debug.Log($"Finger hit detected on child: {child.name}");
                        return true;
                    }
                }
            }
    
            Debug.Log("No valid GameObject found with the name starting with 'Untitled - '.");
            return false;
        }

        return false;
    }

    private void SetCurrentRoute(string routeName)
    {
        var foundRoute = routeList.Find(route => route.Name == routeName);
        if (foundRoute != null)
        {
            currentRoute = foundRoute;
            Debug.Log($"Current route set to: {routeName}");
        }
        else
        {
            Debug.LogWarning($"Route with name '{routeName}' not found.");
        }
    }

    public void DeleteRoute(string routeId)
    {
        var childCount = transform.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i); // Get each child
            if (child.name == routeId)
            {
                var foundRoute = routeList.Find(route => route.Id == routeId);
                routeList.Remove(foundRoute);
                routeManager.DeleteSpawnedRoute(foundRoute);
                Destroy(child.gameObject);
            }
        }
    }

    public void RemoveCurrentRoute()
    {
        currentRoute = null;
    }

    private bool HasDescendantWithNamePrefix(Transform parent, string prefix)
    {
        foreach (Transform child in parent)
        {
            if (child.name.StartsWith(prefix)) return true;
            if (HasDescendantWithNamePrefix(child, prefix)) return true;
        }
        return false;
    }
}