using Mapbox.Unity.Map;
using Oculus.Interaction.Input;
using Oculus.Interaction.PoseDetection;
using UnityEngine;
using static Utils.HandGrabbing;

namespace Gestures
{
    public class MapZoomHandler : MonoBehaviour
    {
        [SerializeField] private AbstractMap mapManager;
        [SerializeField] private FingerFeatureStateProvider leftFingerFeatureStateProvider;
        [SerializeField] private FingerFeatureStateProvider rightFingerFeatureStateProvider;

        [SerializeField] private float zoomSpeed = 1f;
        private int _grabCount;
        private bool _isZooming;
        private float _lastDistance;

        private void Start()
        {
            InitializeReferenceDistance();
        }

        private void LateUpdate()
        {
            var leftHand = leftFingerFeatureStateProvider.Hand;
            var rightHand = rightFingerFeatureStateProvider.Hand;

            if (_isZooming && (!IsGrabbing(leftHand) || !IsGrabbing(rightHand)))
            {
                _isZooming = false;
                return;
            }

            if (!_isZooming && IsGrabbing(leftHand) && IsGrabbing(rightHand))
            {
                _isZooming = true;
                InitializeReferenceDistance();
                return;
            }

            if (!_isZooming) return;

            Pose leftHandPose, rightHandPose;
            leftHand.GetJointPose(HandJointId.HandPalm, out leftHandPose);
            rightHand.GetJointPose(HandJointId.HandPalm, out rightHandPose);
            var leftHandPosition = leftHandPose.position;
            var rightHandPosition = rightHandPose.position;
            var distance = Vector3.Distance(leftHandPosition, rightHandPosition);

            var zoomAmount = zoomSpeed * (distance - _lastDistance);
            var zoom = Mathf.Max(0.0f, Mathf.Min(mapManager.Zoom + zoomAmount, 21.0f));
            mapManager.UpdateMap(mapManager.CenterLatitudeLongitude, zoom);

            _lastDistance = distance;
        }

        private void InitializeReferenceDistance()
        {
            var leftHand = leftFingerFeatureStateProvider.Hand;
            var rightHand = rightFingerFeatureStateProvider.Hand;

            Pose leftHandPose, rightHandPose;
            leftHand.GetJointPose(HandJointId.HandPalm, out leftHandPose);
            rightHand.GetJointPose(HandJointId.HandPalm, out rightHandPose);
            _lastDistance = Vector3.Distance(leftHandPose.position, rightHandPose.position);
        }
    }
}