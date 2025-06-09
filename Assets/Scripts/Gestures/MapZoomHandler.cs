using System;
using Fusion;
using Mapbox.Unity.Map;
using Oculus.Interaction.Input;
using Oculus.Interaction.PoseDetection;
using UnityEngine;
using static Utils.HandGrabbing;

namespace Gestures
{
    public class MapZoomHandler : NetworkBehaviour
    {
        [SerializeField] private AbstractMap mapManager;
        [SerializeField] private FingerFeatureStateProvider leftFingerFeatureStateProvider;
        [SerializeField] private FingerFeatureStateProvider rightFingerFeatureStateProvider;
        [SerializeField] private FloodParentingBehavior floodBehavior;
        [SerializeField] private float zoomSpeed = 1f;
        public bool IsZooming { get; private set; }
        private bool _previousIsZooming;
        private bool _isSpawned;
        private float _lastDistance;
        public Action OnZoom;
        public Action OnZoomBegin;
        public Action OnZoomEnd;

        [Networked]
        [OnChangedRender(nameof(UpdateZoom))]
        private float CurrentZoom { get; set; }

        private void Start()
        {
            mapManager.UpdateMap(mapManager.CenterLatitudeLongitude, mapManager.Zoom);
        }

        private void LateUpdate()
        {
            if (!_isSpawned) return;

            var leftHand = leftFingerFeatureStateProvider.Hand;
            var rightHand = rightFingerFeatureStateProvider.Hand;

            if (IsZooming && (!IsPinching(leftHand) || !IsPinching(rightHand)))
            {
                IsZooming = false;
                InvokeZoomEnd();
                return;
            }

            if (!IsZooming && IsPinching(leftHand) && IsPinching(rightHand))
            {
                IsZooming = true;
                InvokeZoomBegin();
                Object.RequestStateAuthority();
                InitializeReferenceDistance();
            }

            if (!IsZooming) return;

            leftHand.GetJointPose(HandJointId.HandPalm, out var leftHandPose);
            rightHand.GetJointPose(HandJointId.HandPalm, out var rightHandPose);
            var leftHandPosition = leftHandPose.position;
            var rightHandPosition = rightHandPose.position;
            var distance = Vector3.Distance(leftHandPosition, rightHandPosition);

            var zoomAmount = zoomSpeed * (distance - _lastDistance);
            CurrentZoom = Mathf.Max(0.0f, Mathf.Min(mapManager.Zoom + zoomAmount, 21.0f));

            _lastDistance = distance;
        }

        private void InvokeZoomBegin()
        {
            if (_previousIsZooming == IsZooming)
            {
                return;
            }

            _previousIsZooming = IsZooming;
            OnZoomBegin?.Invoke();
        }
        
        private void InvokeZoomEnd()
        {
            if (_previousIsZooming == IsZooming)
            {
                return;
            }

            _previousIsZooming = IsZooming;
            OnZoomEnd?.Invoke();
        }

        public override void Spawned()
        {
            _isSpawned = true;
        }

        private void InitializeReferenceDistance()
        {
            var leftHand = leftFingerFeatureStateProvider.Hand;
            var rightHand = rightFingerFeatureStateProvider.Hand;

            leftHand.GetJointPose(HandJointId.HandPalm, out var leftHandPose);
            rightHand.GetJointPose(HandJointId.HandPalm, out var rightHandPose);
            _lastDistance = Vector3.Distance(leftHandPose.position, rightHandPose.position);
        }

        private void UpdateZoom()
        {
            OnZoom?.Invoke();
            mapManager.UpdateMap(mapManager.CenterLatitudeLongitude, CurrentZoom);
        }
    }
}