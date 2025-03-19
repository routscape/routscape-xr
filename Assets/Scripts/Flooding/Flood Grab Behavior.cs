using Flooding;
using Oculus.Interaction;
using UnityEngine;

public class FloodGrabBehavior : MonoBehaviour
{
    [SerializeField] private GameObject floodCube;
    public float floodStepMultiplier = 0.05f;
    private Vector3 _lastPinchPos;      // <- keep track of last pinch position
    private bool _hasLastPinchPos = false; // because you only have a valid "previous" after one frame
    private int _grabCount = 0;
    private float currentHeight = 1;

    public void OnSelect(PointerEvent pointerEvent)
    {
        var pinchArea = pointerEvent.Data as GameObject;
        if (pinchArea == null)
        {
            Debug.LogError("[FLOOD CUBE] pinchArea expected in Data property of hand");
            return;
        }

        if (++_grabCount != 1) return;

        // Initialize last pinch pos so we can compare next frame
        _lastPinchPos = pinchArea.transform.position;
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
        if (_grabCount != 1) return;
        var pinchArea = pointerEvent.Data as GameObject;
        if (pinchArea == null || !_hasLastPinchPos) return;

        // Compare new pinch position to last frame
        var newPinchPos = pinchArea.transform.position;

        // If there's no movement (or very tiny), do nothing:
        float deltaY = newPinchPos.y - _lastPinchPos.y;
        if (Mathf.Abs(deltaY) < 0.0001f)
        {
            // No real movement, early-out
            return;
        }

        // Otherwise, we move the flood
        float currentDelta = deltaY * floodStepMultiplier;
        float newHeight = currentHeight + currentDelta;
        currentHeight = newHeight;

        Debug.Log("NEW HEIGHT " + newHeight);
        floodCube.GetComponent<MeshFilter>()
            .mesh.RecalculateMeshByBounds(new Vector3(1, 1, newHeight));

        // update for next frame
        _lastPinchPos = newPinchPos;
    }
}