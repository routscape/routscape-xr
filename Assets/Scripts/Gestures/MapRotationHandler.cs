using Fusion;
using UnityEngine;

namespace Gestures
{
    public class MapRotationHandler : NetworkBehaviour
    {
        [SerializeField] private Transform monitoredTransform;

        private bool _isSpawned;

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
    }
}