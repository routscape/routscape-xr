using Oculus.Interaction;
using UnityEngine;

public class CenterBetweenRays : MonoBehaviour
{
    private Vector3 _position1;
    private bool _position1Set;

    private Vector3 _position2;
    private bool _position2Set;

    public void OnSelect(PointerEvent pointerEvent)
    {
        if (pointerEvent.Type != PointerEventType.Select)
        {
            Debug.LogError("[CenterBetweenRays] Unexpected pointer event type: " + pointerEvent.Type);
            return;
        }

        var rayInteractor = (RayInteractor)pointerEvent.Data;

        // Fill in the positions of the two rays
        if (!_position1Set)
        {
            Debug.Log("[CenterBetweenRays] Setting position 1 at " + rayInteractor.End);
            _position1Set = true;
            _position1 = rayInteractor.End;
            return;
        }

        if (!_position2Set)
        {
            Debug.Log("[CenterBetweenRays] Setting position 2 at " + rayInteractor.End);
            _position2Set = true;
            _position2 = rayInteractor.End;
        }

        // If both positions are filled, calculate the center point
        if (_position1Set && _position2Set)
        {
            var center = (_position1 + _position2) / 2;
            transform.position = center;
            Debug.Log("[CenterBetweenRays] Centering at " + center);

            // Reset the positions
            _position1Set = false;
            _position2Set = false;
        }
    }

    public void OnDeselect(PointerEvent pointerEvent)
    {
        if (pointerEvent.Type != PointerEventType.Unselect)
        {
            Debug.LogError("[CenterBetweenRays] Unexpected pointer event type: " + pointerEvent.Type);
            return;
        }

        if (_position1Set && !_position2Set)
        {
            Debug.Log("[CenterBetweenRays] Resetting position 1");
            _position1Set = false;
        }
    }
}