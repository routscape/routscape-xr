using Flooding;
using Fusion;
using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Oculus.Interaction;
using UnityEngine;

public class FloodGrabBehavior : MonoBehaviour
{
    [SerializeField] private GameObject floodCube;
    [SerializeField] private AbstractMap mapManager;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private MeshRenderer meshRenderer;
    
    public float floodStepMultiplier;
    public float currentFloodLevel; 

    private int _grabCount;
    private Vector3 _lastPinchPos; 
    private double _currentBounds  = 1;

    void Start()
    {
        boxCollider = transform.parent.GetComponent<BoxCollider>();
        mapManager.OnUpdated += CalculateNewBounds;
        CalculateNewBounds();
    }

    public void OnSelect(PointerEvent pointerEvent)
    {
        var go= pointerEvent.Data as GameObject;

        if (go == null)
        {
            Debug.LogError("[FLOOD CUBE] interactor expected in Data property of hand!");
            return;
        }
        

        var pinchArea = GetPinchArea(gameObject);

        if (++_grabCount != 1) return;

        _lastPinchPos = pinchArea;
    }

    public void OnDeselect(PointerEvent pointerEvent)
    {
        --_grabCount;
        Debug.Log("Flood Deselected");
    }

    public void OnMove(PointerEvent pointerEvent)
    {
        if (_grabCount != 1) return;
        var go = pointerEvent.Data as GameObject;
        var pinchArea = GetPinchArea(go);

        var newPinchPos = pinchArea;
        var deltaY = newPinchPos.y - _lastPinchPos.y;
        if (Mathf.Abs(deltaY) < 0.0001f) return;

        var currentDelta = deltaY * floodStepMultiplier;
        var newFloodLevel = currentFloodLevel + currentDelta;
        currentFloodLevel = newFloodLevel;
        CalculateNewBounds();
        
        _lastPinchPos = newPinchPos;
    }

    public void SetFloodLevel(float centimeters)
    {
        currentFloodLevel = centimeters;
        CalculateNewBounds();
        MoveFlood();
    }

    public void Show()
    {
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
        boxCollider.enabled = false;
    }
    
    private Vector3 GetPinchArea(GameObject go)
    {
        if (go.tag.Contains("left")) return GameObject.FindWithTag("left pinch area").transform.position;

        return GameObject.FindWithTag("right pinch area").transform.position;
    }

    private void CalculateNewBounds()
    {
        RectD referenceTileRect = Conversions.TileBounds(TileCover.CoordinateToTileId(mapManager.CenterLatitudeLongitude, mapManager.AbsoluteZoom));
        double zoomDifference = mapManager.Zoom - mapManager.AbsoluteZoom;
        double floodLevelInMeters = currentFloodLevel / 100;
        double unitsPerMeter = (mapManager.Options.scalingOptions.unityTileSize / referenceTileRect.Size.x) * Mathd.Pow(2d, zoomDifference);
        _currentBounds = floodLevelInMeters * unitsPerMeter;
    }
    
    private void MoveFlood()
    {
        meshFilter.mesh.RecalculateMeshByBounds(new Vector3(1, 1, (float)_currentBounds));
        var meshBounds = meshFilter.mesh.bounds;
        boxCollider.center = meshBounds.center;
        boxCollider.size = meshBounds.size;
    }
}