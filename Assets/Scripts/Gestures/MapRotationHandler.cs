using Fusion;
using Oculus.Interaction;
using UnityEngine;

namespace Gestures
{
    public class MapRotationHandler : NetworkBehaviour
    {
        [SerializeField] private RayInteractor leftRayInteractor;
        [SerializeField] private RayInteractor rightRayInteractor;
        [SerializeField] private Transform monitoredTransform;

        private bool _isSpawned;

        private Vector3 _leftPosition;
        private bool _leftPositionSet;

        private Vector3 _rightPosition;
        private bool _rightPositionSet;

        [Networked]
        [OnChangedRender(nameof(UpdateRotation))]
        private Quaternion CurrentRotation { get; set; }

        private void FixedUpdate()
        {
            if (!_isSpawned) return;

            if (!monitoredTransform.rotation.Equals(CurrentRotation))
                CurrentRotation = monitoredTransform.rotation;
        }

        public override void Spawned()
        {
            _isSpawned = true;
        }

        private void UpdateRotation()
        {
            if (!Object.HasStateAuthority)
            {
                Debug.Log("[MapRotationHandler] Updating rotation to " + CurrentRotation);
                monitoredTransform.rotation = CurrentRotation;
            }
        }

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
                Object.RequestStateAuthority();

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