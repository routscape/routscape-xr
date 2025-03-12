using System.Collections.Generic;
using System.Linq;
using Fusion;
using Mapbox.Unity.Map;
using UnityEngine;

public class XRRouteDrawer : NetworkBehaviour
{
    public int numTotalRoutes;

    [SerializeField] private LayerMask mapboxLayer;
    [SerializeField] private UserInterfaceManagerScript userInterfaceManagerScript;
    [SerializeField] private OVRHand rightHand;
    [SerializeField] private OVRSkeleton.BoneId selectedFinger = OVRSkeleton.BoneId.Hand_IndexTip;
    [SerializeField] private RouteManager routeManager;
    public float minDistanceBetweenPoints = 0.003f * 10000f; // Minimum distance to register a new point
    public float rayDistance = 0.05f;
    private readonly ColorType initialColor = ColorType.Blue;

    private readonly List<Route> routeList = new();

    private AbstractMap _mapManager;
    private Route currentRoute;
    private Vector3 lastPoint = Vector3.zero;

    [Networked]
    [OnChangedRender(nameof(AddPoint))]
    private Vector3 PointToAdd { get; set; }

    private void Start()
    {
        _mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
        minDistanceBetweenPoints = 0.003f * 10000f;
    }

    private void FixedUpdate()
    {
        Vector3 hitPoint;
        if (GetFingerHitPoint(out hitPoint)) // Ensure raycast hits something
        {
            var distance = Vector3.Distance(hitPoint, lastPoint);
            distance *= 10000; // floating comparison sucks
            Debug.Log("Distance " + distance + " vs " + minDistanceBetweenPoints + " " +
                      (distance > minDistanceBetweenPoints));
            if (distance > minDistanceBetweenPoints
                && userInterfaceManagerScript.mode == 1)
            {
                PointToAdd = hitPoint; // Add point only if moved significantly
                lastPoint = hitPoint;
            }
        }
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
            adjustedPoint = hit.point + Vector3.up * 0.01f; // Slight offset
            Debug.Log($"Finger hit detected at: {adjustedPoint}");
            return true;
        }

        return false;
    }

    private bool GetControllerHitPoint(out Vector3 adjustedPoint)
    {
        // Get the controller's position and rotation
        var origin = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        var rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // XR Simulator fallback
        if (origin == Vector3.zero) origin = transform.position;
        if (rotation == Quaternion.identity) rotation = transform.rotation;

        // Calculate the direction the controller is pointing
        var direction = rotation * Vector3.forward;

        // Debug the ray to visualize it in Scene View
        Debug.DrawRay(origin, direction * 50f, Color.green, 1f);

        // Raycast to detect where the controller is pointing
        if (Physics.Raycast(origin, direction, out var hit, 50f, mapboxLayer))
        {
            adjustedPoint = hit.point + Vector3.up * 0.01f; // Slightly above surface
            Debug.Log($"Hit detected at: {adjustedPoint}");
            return true;
        }

        adjustedPoint = Vector3.zero;
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

    public void UpdateRoute(string routeName)
    {
        var childCount = transform.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i); // Get each child
            if (child.name == routeName)
            {
                var foundRoute = routeList.Find(route => route.Name == routeName);

                var lineRenderer = child.GetComponent<LineRenderer>();
                lineRenderer.startColor = foundRoute.Color; // Set the starting color to red
                lineRenderer.endColor = foundRoute.Color;
            }
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
}