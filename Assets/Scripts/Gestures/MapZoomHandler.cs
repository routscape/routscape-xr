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
        private bool _isSpawned;
        private bool _isZooming;
        private float _lastDistance;
        public bool CanZoom { private get; set; } = true;
        public Action OnZoom;
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
            if (!CanZoom) return;
            if (!_isSpawned) return;

            var leftHand = leftFingerFeatureStateProvider.Hand;
            var rightHand = rightFingerFeatureStateProvider.Hand;

            if (_isZooming && (!IsPinching(leftHand) || !IsPinching(rightHand)))
            {
                //minus gesture
                OnZoomEnd?.Invoke();
                _isZooming = false;
                return;
            }

            if (!_isZooming && IsPinching(leftHand) && IsPinching(rightHand))
            {
                //gesture in action
                _isZooming = true;
                Object.RequestStateAuthority();
                InitializeReferenceDistance();
            }

            if (!_isZooming) return;

            leftHand.GetJointPose(HandJointId.HandPalm, out var leftHandPose);
            rightHand.GetJointPose(HandJointId.HandPalm, out var rightHandPose);
            var leftHandPosition = leftHandPose.position;
            var rightHandPosition = rightHandPose.position;
            var distance = Vector3.Distance(leftHandPosition, rightHandPosition);

            var zoomAmount = zoomSpeed * (distance - _lastDistance);
            CurrentZoom = Mathf.Max(0.0f, Mathf.Min(mapManager.Zoom + zoomAmount, 21.0f));

            _lastDistance = distance;
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