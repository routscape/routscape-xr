using Fusion;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Oculus.Interaction;
using UnityEngine;

namespace Gestures
{
    public class MapMovementHandler : NetworkBehaviour
    {
        [SerializeField] private AbstractMap mapManager;

        private int _grabCount;
        private int _interactorId = -1;
        private Vector3 _referencePosition;

        [Networked]
        [OnChangedRender(nameof(UpdateLatLon))]
        private Vector3 CurrentLatLong { get; set; }

        public void OnSelect(PointerEvent pointerEvent)
        {
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

            if (++_grabCount != 1) return;

            var rayInteractor = gameObject.GetComponent<RayInteractor>();
            Object.RequestStateAuthority();
            _interactorId = pointerEvent.Identifier;
            _referencePosition = rayInteractor.End;
        }

        public void OnDeselect(PointerEvent pointerEvent)
        {
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

            _interactorId = -1;
            --_grabCount;
        }

        public void OnMove(PointerEvent pointerEvent)
        {
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

            if (pointerEvent.Identifier != _interactorId) return;
            if (_grabCount != 1) return;

            var delta = rayInteractor.End - _referencePosition;

            _referencePosition = rayInteractor.End;

            var newLatLong = mapManager.WorldToGeoPosition(mapManager.Root.position - delta);
            CurrentLatLong = newLatLong.ToVector3xz();
        }

        private void UpdateLatLon()
        {
            mapManager.UpdateMap(CurrentLatLong.ToVector2d());
        }
    }
}