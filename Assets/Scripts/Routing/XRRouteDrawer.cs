using UnityEngine;
using System.Collections.Generic;

public class XRRouteDrawer : MonoBehaviour
{
    [SerializeField] private LayerMask mapboxLayer;
    [SerializeField] private Color initialColor = Color.red; // Default color is red
    private List<Route> routeList = new List<Route>();
    private List<Vector3> routePoints = new List<Vector3>();
    private bool isDrawing = false;
    
    void Start()
    {
        CreateNewLine();
    }

    void Update()
    {
        // If trigger is pressed then start/continue drawing
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            StartDrawing();
        }

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) && isDrawing)
        {
            Vector3 hitPoint;
            if (GetControllerHitPoint(out hitPoint))
            {
                AddPoint(hitPoint);
            }
        }

        // If trigger is released then Stop drawing
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            StopDrawing();
        }
    }
    
    [ContextMenu("Create New Line")]
    public void CreateNewLine()
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
        
        Route newRoute = new Route("Line " + routeList.Count, newLineRenderer, initialColor);
        routeList.Add(newRoute);
    }

    private void StartDrawing()
    {
        isDrawing = true;
        Debug.Log("Drawing started.");
    }

    private void StopDrawing()
    {
        isDrawing = false;
        Debug.Log($"Drawing stopped. Points: {routePoints.Count}");
    }

    private void AddPoint(Vector3 newPoint)
    {
        Route currentRoute = routeList[routeList.Count - 1];
        currentRoute.AddPoint(newPoint);
    }
    
    private bool GetControllerHitPoint(out Vector3 adjustedPoint)
    {
        Vector3 origin = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        Vector3 direction = rotation * Vector3.forward;

        Debug.DrawRay(origin, direction * 50f, Color.green, 1f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, 10f, mapboxLayer))
        {
            adjustedPoint = hit.point + Vector3.up * 0.01f;
            return true;
        }

        adjustedPoint = Vector3.zero;
        return false;
    }
}
