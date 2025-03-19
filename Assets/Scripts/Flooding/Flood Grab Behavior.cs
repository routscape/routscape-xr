using Flooding;
using Fusion;
using Oculus.Interaction;
using UnityEngine;

public class FloodGrabBehavior : NetworkBehaviour
{
    [SerializeField] private GameObject floodCube;
    public float floodStepMultiplier = 0.05f;
    private BoxCollider _boxCollider;

    private int _grabCount;
    private bool _hasLastPinchPos; // because you only have a valid "previous" after one frame
    private Vector3 _lastPinchPos; // <- keep track of last pinch position

    [Networked]
    [OnChangedRender(nameof(MoveFlood))]
    private float CurrentHeight { get; set; } = 1;

    private void Start()
    {
        _boxCollider = transform.parent.GetComponent<BoxCollider>();
    }

    public void OnSelect(PointerEvent pointerEvent)
    {
        var gameObject = pointerEvent.Data as GameObject;

        if (gameObject == null)
        {
            Debug.LogError("[FLOOD CUBE] interactor expected in Data property of hand!");
            return;
        }

        Object.RequestStateAuthority();

        var pinchArea = GetPinchArea(gameObject);

        if (++_grabCount != 1) return;

        // Initialize last pinch pos so we can compare next frame
        _lastPinchPos = pinchArea;
        _hasLastPinchPos = true;

        Debug.Log("Flood Selected");
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
        var gameObject = pointerEvent.Data as GameObject;
        var pinchArea = GetPinchArea(gameObject);

        var newPinchPos = pinchArea;
        var deltaY = newPinchPos.y - _lastPinchPos.y;
        if (Mathf.Abs(deltaY) < 0.0001f) return;

        var currentDelta = deltaY * floodStepMultiplier;
        var newHeight = CurrentHeight + currentDelta;
        CurrentHeight = newHeight;

        Debug.Log("NEW HEIGHT " + newHeight);

        _lastPinchPos = newPinchPos;
    }

    private Vector3 GetPinchArea(GameObject gameObject)
    {
        if (gameObject.tag.Contains("left")) return GameObject.FindWithTag("left pinch area").transform.position;

        return GameObject.FindWithTag("right pinch area").transform.position;
    }

    private void MoveFlood()
    {
        var meshFilter = floodCube.GetComponent<MeshFilter>();
        meshFilter.mesh.RecalculateMeshByBounds(new Vector3(1, 1, CurrentHeight));
        var meshBounds = meshFilter.mesh.bounds;
        _boxCollider.center = meshBounds.center;
        _boxCollider.size = meshBounds.size;
    }
}