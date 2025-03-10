using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mapbox.Unity.Map;
using TMPro;
using UnityEngine.UI;

public class XRRouteDrawer : MonoBehaviour
{
    public int numTotalRoutes = 0;
    
    [SerializeField] private LayerMask mapboxLayer;
    private ColorType initialColor = ColorType.Blue;
	[SerializeField] private UserInterfaceManagerScript userInterfaceManagerScript;
	[SerializeField] private OVRHand rightHand;
	[SerializeField] private OVRSkeleton.BoneId selectedFinger = OVRSkeleton.BoneId.Hand_IndexTip;
    [SerializeField] private RouteManager routeManager;
    
    private List<Route> routeList = new List<Route>();
    private Route currentRoute;
    private Vector3 lastPoint = Vector3.zero;
    private float minDistanceBetweenPoints = 0.06f; // Minimum distance to register a new point

    private AbstractMap _mapManager;

    void Start()
    {
        _mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
    }
    void Update()
    {
        Vector3 hitPoint;
        if (GetFingerHitPoint(out hitPoint)) // Ensure raycast hits something
        {
            if (currentRoute == null) return;
    
            if (Vector3.Distance(hitPoint, lastPoint) > minDistanceBetweenPoints
				&& userInterfaceManagerScript.currentActiveRoute != null)
            {
                AddPoint(hitPoint); // Add point only if moved significantly
                lastPoint = hitPoint;
            }
        }
    }

    public Route CreateNewLine()
    {
        string name = "Route - " + numTotalRoutes;
        GameObject newLineObj = new GameObject(name);
        newLineObj.transform.parent = transform;
        LineRenderer newLineRenderer = newLineObj.AddComponent<LineRenderer>();
        newLineRenderer.positionCount = 0;
        newLineRenderer.startWidth = 0.01f;
        newLineRenderer.endWidth = 0.01f;
        newLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        newLineRenderer.startColor = ColorHexCodes.GetColor(initialColor);
        newLineRenderer.endColor = ColorHexCodes.GetColor(initialColor);

        Route newRoute = new Route(name, newLineRenderer, initialColor);
        newRoute.prefab = newLineObj;
        routeList.Add(newRoute);
        routeManager.AddSpawnedRoute(newRoute);
        SetCurrentRoute(name);
        Debug.Log("CreateNewLine");
        numTotalRoutes++;
        return newRoute;
    }

    private void AddPoint(Vector3 newPoint)
    {
        if (currentRoute == null) return; // Safety check
        currentRoute.AddPoint(newPoint, _mapManager);
        Debug.Log($"Point added: {newPoint}");
    }

	private bool GetFingerHitPoint(out Vector3 adjustedPoint)
    {
        adjustedPoint = Vector3.zero;
    
        if (rightHand == null)
        {
            Debug.LogWarning("Right hand not assigned!");
            return false;
        }
    
        OVRSkeleton skeleton = rightHand.GetComponent<OVRSkeleton>();
        if (skeleton == null || !skeleton.IsDataValid || !skeleton.IsDataHighConfidence || skeleton.Bones == null || skeleton.Bones.Count == 0)
        {
            Debug.LogWarning("Skeleton data is not valid or not initialized!");
            return false;
        }
    
        // **Find the fingertip bone using the selectedFinger value**
        OVRBone fingertip = skeleton.Bones.FirstOrDefault(b => b.Id == selectedFinger);
    
        if (fingertip == null)
        {
            Debug.LogWarning($"Selected fingertip {selectedFinger} not found!");
            return false;
        }
    
        // Use fingertip position
        Vector3 fingerPosition = fingertip.Transform.position;
    
        Debug.DrawRay(fingerPosition, Vector3.up * 0.05f, Color.red, 0.1f);
        
        // **Raycast downward to detect the map**
        if (Physics.Raycast(fingerPosition, Vector3.down, out RaycastHit hit, 0.02f, mapboxLayer))
        {
            adjustedPoint = hit.point + Vector3.up * 0.01f;  // Slight offset
            Debug.Log($"Finger hit detected at: {adjustedPoint}");
            return true;
        }
    
        return false;
    }


    private bool GetControllerHitPoint(out Vector3 adjustedPoint)
    {
        // Get the controller's position and rotation
        Vector3 origin = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
    
        // XR Simulator fallback
        if (origin == Vector3.zero) origin = transform.position;
        if (rotation == Quaternion.identity) rotation = transform.rotation;
    
        // Calculate the direction the controller is pointing
        Vector3 direction = rotation * Vector3.forward;
    
        // Debug the ray to visualize it in Scene View
        Debug.DrawRay(origin, direction * 50f, Color.green, 1f);
    
        // Raycast to detect where the controller is pointing
        if (Physics.Raycast(origin, direction, out RaycastHit hit, 50f, mapboxLayer)) 
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

	public void UpdateRoute(string routeName)
	{
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i); // Get each child
			if (child.name == routeName)
			{
				Route foundRoute = routeList.Find(route => route.Name == routeName);

				LineRenderer lineRenderer = child.GetComponent<LineRenderer>();
				lineRenderer.startColor = foundRoute.Color;  // Set the starting color to red
            	lineRenderer.endColor = foundRoute.Color;
			}
        }
	}

    public void DeleteRoute(string routeName)
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i); // Get each child
            if (child.name == routeName)
            {
                Route foundRoute = routeList.Find(route => route.Name == routeName);
                routeList.Remove(foundRoute);

                Destroy(child.gameObject);
            }
        }
    }
    
    public void RemoveCurrentRoute()
    {
        currentRoute = null;
    }
}