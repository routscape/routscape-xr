using Flooding;
using Fusion;
using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Oculus.Interaction;
using UnityEngine;

public class FloodGrabBehavior : NetworkBehaviour
{
    [SerializeField] private GameObject floodCube;
    [SerializeField] private AbstractMap mapManager;
    [SerializeField] private MeshFilter meshFilter;
    public float floodStepMultiplier;
    public float currentFloodLevel; 

    private BoxCollider _boxCollider;
    private int _grabCount;
    private Vector3 _lastPinchPos; 

    [Networked]
    [OnChangedRender(nameof(MoveFlood))]
    private double CurrentBounds { get; set; } = 1;

    public override void Spawned()
    {
        _boxCollider = transform.parent.GetComponent<BoxCollider>();
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

        Object.RequestStateAuthority();

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
        if (!Object.HasStateAuthority) return;

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
        CurrentBounds = floodLevelInMeters * unitsPerMeter;
    }
    
    private void MoveFlood()
    {
        meshFilter.mesh.RecalculateMeshByBounds(new Vector3(1, 1, (float)CurrentBounds));
        var meshBounds = meshFilter.mesh.bounds;
        _boxCollider.center = meshBounds.center;
        _boxCollider.size = meshBounds.size;
    }
}