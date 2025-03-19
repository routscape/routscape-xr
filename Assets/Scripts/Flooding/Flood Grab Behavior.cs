using Flooding;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using Oculus.Interaction.Input.Compatibility.OVR;
using UnityEngine;
using Handedness = Oculus.Interaction.Input.Handedness;

public class FloodGrabBehavior : MonoBehaviour
{
    [SerializeField] private GameObject floodCube;
    public float floodStepMultiplier = 0.05f;
    private Vector3 _lastPinchPos;      // <- keep track of last pinch position
    private bool _hasLastPinchPos = false; // because you only have a valid "previous" after one frame
    private int _grabCount = 0;
    private float _currentHeight = 1;
    
    public void OnSelect(PointerEvent pointerEvent)
    {
        var gameObject = (pointerEvent.Data as GameObject);

        if (gameObject == null)
        {
            Debug.LogError("[FLOOD CUBE] pinchArea expected in Data property of hand");
            return;
        }

        var pinchArea = gameObject.transform.position;
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
        if (_grabCount != 1) return;
        var gameObject = (pointerEvent.Data as GameObject);
        var pinchArea = gameObject.transform.position;
        
        var newPinchPos = pinchArea;
        float deltaY = newPinchPos.y - _lastPinchPos.y;
        if (Mathf.Abs(deltaY) < 0.0001f)
        {
            return;
        }

        float currentDelta = deltaY * floodStepMultiplier;
        float newHeight = _currentHeight + currentDelta;
        _currentHeight = newHeight;

        Debug.Log("NEW HEIGHT " + newHeight);
        floodCube.GetComponent<MeshFilter>()
            .mesh.RecalculateMeshByBounds(new Vector3(1, 1, newHeight));
        _lastPinchPos = newPinchPos;
    }
}