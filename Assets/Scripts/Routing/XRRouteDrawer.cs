using UnityEngine;
using System.Collections.Generic;

public class XRRouteDrawer : MonoBehaviour
{
    [SerializeField] private LayerMask mapboxLayer;
    [SerializeField] private Color initialColor = Color.red; // Default color is red
    private List<Route> routeList = new List<Route>();
    private bool isDrawing = false;
    private Route currentRoute;

    void Update()
    {
        // Detect a single poke (trigger press)
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            Vector3 hitPoint;
            if (GetControllerHitPoint(out hitPoint))
            {
                AddPoint(hitPoint);  // Register a point only on trigger press
            }
        }

        // Stop drawing if the user releases the trigger
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            StopDrawing();
        }
    }
    
    public Route CreateNewLine(string name)
    {
        GameObject newLineObj = new GameObject("Route");
        newLineObj.transform.parent = transform;
        LineRenderer newLineRenderer = newLineObj.AddComponent<LineRenderer>();
        newLineRenderer.positionCount = 0;
        newLineRenderer.startWidth = 0.01f;
        newLineRenderer.endWidth = 0.01f;
        newLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        newLineRenderer.startColor = initialColor;
        newLineRenderer.endColor = initialColor;
        
        Route newRoute = new Route(name, newLineRenderer, initialColor);
        routeList.Add(newRoute);
        SetCurrentRoute(name);
        Debug.Log("CreateNewLine");
        return newRoute;
    }

    private void AddPoint(Vector3 newPoint)
    {
        Debug.Log(currentRoute.Name);
        if (routeList.Count == 0) return; // Safety check
        currentRoute.AddPoint(newPoint);

        Debug.Log($"Point added: {newPoint}");
    }

    private void StopDrawing()
    {
        isDrawing = false;
        Debug.Log("Drawing stopped.");
    }

    private bool GetControllerHitPoint(out Vector3 adjustedPoint)
    {
        Vector3 origin = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        Vector3 direction = rotation * Vector3.forward;

        Debug.DrawRay(origin, direction * 50f, Color.green, 1f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, 10f, mapboxLayer))
        {
            adjustedPoint = hit.point + Vector3.up * 0.01f; // Slightly offset above the surface
            return true;
        }

        adjustedPoint = Vector3.zero;
        return false;
    }
    
    private void SetCurrentRoute(string routeName)
    {
        Route foundRoute = routeList.Find(route => route.Name == routeName);
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
}
