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
        private bool _leftPositionSet;
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
                //gesture in action
                Debug.Log("[MapRotationHandler] Updating rotation to " + CurrentRotation);
                monitoredTransform.rotation = CurrentRotation;
            }
        }

        public void OnSelect(PointerEvent pointerEvent)
        {
            if (pointerEvent.Type != PointerEventType.Select)
            {
                Debug.LogWarning("[MapRotationHandler] Ignoring due to unexpected pointer event type: " +
                                 pointerEvent.Type);
                return;
            }

            var gameObject = pointerEvent.Data as GameObject;

            if (gameObject == null)
            {
                Debug.LogError("[MapMovementHandler] Need to assign interactor game object to data!");
                return;
            }
            if (!gameObject.tag.Contains("ray interactor"))
            {
                Debug.LogErrorFormat("[MapMovementHandler] Expected RayInteractor but got {0}",
                    pointerEvent.Data.GetType().Name);
                return;
            }
            var rayInteractor = gameObject.GetComponent<RayInteractor>();

            if (rayInteractor == leftRayInteractor) _leftPositionSet = true;
            if (rayInteractor == rightRayInteractor) _rightPositionSet = true;

            if (_leftPositionSet && _rightPositionSet) Object.RequestStateAuthority();
        }

        public void OnDeselect(PointerEvent pointerEvent)
        {
            if (pointerEvent.Type != PointerEventType.Unselect)
            {
                Debug.LogWarning("[MapRotationHandler] Ignoring due to unexpected pointer event type: " +
                                 pointerEvent.Type);
                return;
            }

            var gameObject = pointerEvent.Data as GameObject;

            if (gameObject == null)
            {
                Debug.LogError("[MapMovementHandler] Need to assign interactor game object to data!");
                return;
            }
            if (!gameObject.tag.Contains("ray interactor"))
            {
                Debug.LogErrorFormat("[MapMovementHandler] Expected RayInteractor but got {0}",
                    pointerEvent.Data.GetType().Name);
                return;
            }
            var rayInteractor = gameObject.GetComponent<RayInteractor>();

            if (rayInteractor == leftRayInteractor) _leftPositionSet = false;
            if (rayInteractor == rightRayInteractor) _rightPositionSet = false;
            //gesture end
        }
    }
}