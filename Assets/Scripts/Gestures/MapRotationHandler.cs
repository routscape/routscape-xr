using Oculus.Interaction;
using Oculus.Interaction.PoseDetection;
using UnityEngine;
using static Utils.HandGrabbing;

namespace Gestures
{
    public class MapRotationHandler : MonoBehaviour
    {
        [SerializeField] private bool restrictToPinch;
        [SerializeField] private bool restrictToGrab;
        [SerializeField] private FingerFeatureStateProvider leftFingerFeatureStateProvider;
        [SerializeField] private FingerFeatureStateProvider rightFingerFeatureStateProvider;

        [SerializeField] private RayInteractor leftRayInteractor;
        [SerializeField] private RayInteractor rightRayInteractor;

        private Vector3 _leftPosition;
        private bool _leftPositionSet;

        private Vector3 _rightPosition;
        private bool _rightPositionSet;

        public void OnSelect(PointerEvent pointerEvent)
        {
            if (pointerEvent.Type != PointerEventType.Select)
            {
                Debug.LogError("[MapRotationHandler] Unexpected pointer event type: " + pointerEvent.Type);
                return;
            }

            var rayInteractor = pointerEvent.Data as RayInteractor;
            if (rayInteractor == null)
            {
                Debug.LogError("[MapRotationHandler] Unexpected pointer event data type: " +
                               pointerEvent.Data.GetType());
                return;
            }

            if (restrictToPinch && (!IsPinching(leftFingerFeatureStateProvider.Hand) ||
                                    !IsPinching(rightFingerFeatureStateProvider.Hand)))
            {
                Debug.Log("[MapRotationHandler] Not pinching with both hands");
                return;
            }

            if (restrictToGrab && (!IsGrabbing(leftFingerFeatureStateProvider.Hand) ||
                                   !IsGrabbing(rightFingerFeatureStateProvider.Hand)))
            {
                Debug.Log("[MapRotationHandler] Not grabbing with both hands");
                return;
            }

            // Fill in the positions of the two rays
            if (rayInteractor == leftRayInteractor)
            {
                Debug.Log("[MapRotationHandler] Setting left position at " + rayInteractor.End);
                _leftPositionSet = true;
                _leftPosition = rayInteractor.End;
            }

            if (rayInteractor == rightRayInteractor)
            {
                Debug.Log("[MapRotationHandler] Setting right position at " + rayInteractor.End);
                _rightPositionSet = true;
                _rightPosition = rayInteractor.End;
            }

            // If both positions are filled, calculate the center point
            if (_leftPositionSet && _rightPositionSet)
            {
                var center = (_leftPosition + _rightPosition) / 2;
                transform.position = center;
                Debug.Log("[MapRotationHandler] Centering at " + center);
            }
        }

        public void OnDeselect(PointerEvent pointerEvent)
        {
            if (pointerEvent.Type != PointerEventType.Unselect)
            {
                Debug.LogError("[MapRotationHandler] Unexpected pointer event type: " + pointerEvent.Type);
                return;
            }

            var rayInteractor = pointerEvent.Data as RayInteractor;
            if (rayInteractor == null)
            {
                Debug.LogError("[MapRotationHandler] Unexpected pointer event data type: " +
                               pointerEvent.Data.GetType());
                return;
            }

            if (rayInteractor == leftRayInteractor)
            {
                Debug.Log("[MapRotationHandler] Resetting left position");
                _leftPositionSet = false;
            }

            if (rayInteractor == rightRayInteractor)
            {
                Debug.Log("[MapRotationHandler] Resetting right position");
                _rightPositionSet = false;
            }
        }
    }
}