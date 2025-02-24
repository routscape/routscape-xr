using Mapbox.Unity.Map;
using Oculus.Interaction;
using UnityEngine;

namespace Gestures
{
    public class MapMovementHandler : MonoBehaviour
    {
        [SerializeField] private AbstractMap mapManager;

        private int _grabCount;
        private int _interactorId = -1;
        private Vector3 _referencePosition;

        public void OnSelect(PointerEvent pointerEvent)
        {
            var rayInteractor = pointerEvent.Data as RayInteractor;

            if (rayInteractor == null)
            {
                Debug.LogErrorFormat("[MapMovementHandler] Expected RayInteractor but got {0}",
                    pointerEvent.Data.GetType().Name);
                return;
            }

            if (++_grabCount != 1) return;

            _interactorId = pointerEvent.Identifier;
            _referencePosition = rayInteractor.End;
        }

        public void OnDeselect(PointerEvent pointerEvent)
        {
            var rayInteractor = pointerEvent.Data as RayInteractor;

            if (rayInteractor == null)
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
            var rayInteractor = pointerEvent.Data as RayInteractor;

            if (rayInteractor == null)
            {
                Debug.LogErrorFormat("[MapMovementHandler] Expected RayInteractor but got {0}",
                    pointerEvent.Data.GetType().Name);
                return;
            }

            if (pointerEvent.Identifier != _interactorId) return;
            if (_grabCount != 1) return;

            var delta = rayInteractor.End - _referencePosition;

            _referencePosition = rayInteractor.End;

            var newLatLong = mapManager.WorldToGeoPosition(mapManager.Root.position - delta);
            mapManager.UpdateMap(newLatLong);
        }
    }
}