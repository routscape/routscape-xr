using Mapbox.Unity.Map;
using Oculus.Interaction.Input;
using Oculus.Interaction.PoseDetection;
using UnityEngine;

public class MapZoomHandler : MonoBehaviour
{
    [SerializeField] private AbstractMap mapManager;
    [SerializeField] private FingerFeatureStateProvider leftFingerFeatureStateProvider;
    [SerializeField] private FingerFeatureStateProvider rightFingerFeatureStateProvider;

    [SerializeField] private float zoomSpeed = 0.1f;
    private bool _isGrabbingLeft;
    private bool _isGrabbingRight;
    private Vector3 _leftHandPosition;
    private Vector3 _rightHandPosition;

    private void FixedUpdate()
    {
        if (!_isGrabbingLeft || !_isGrabbingRight) return;

        var distance = Vector3.Distance(_leftHandPosition, _rightHandPosition);
        var zoomAmount = distance * zoomSpeed;

        var zoom = Mathf.Max(0.0f, Mathf.Min(mapManager.Zoom + zoomAmount, 21.0f));

        mapManager.UpdateMap(mapManager.CenterLatitudeLongitude, zoom);
    }

    public void OnLeftGrab()
    {
        _isGrabbingLeft = true;

        Pose leftHandPose;
        var leftHand = leftFingerFeatureStateProvider.Hand;
        leftHand.GetJointPose(HandJointId.HandPalm, out leftHandPose);
        _leftHandPosition = leftHandPose.position;
    }

    public void OnLeftUngrab()
    {
        _isGrabbingLeft = false;
    }

    public void OnRightGrab()
    {
        _isGrabbingRight = true;

        Pose rightHandPose;
        var rightHand = rightFingerFeatureStateProvider.Hand;
        rightHand.GetJointPose(HandJointId.HandPalm, out rightHandPose);
        _rightHandPosition = rightHandPose.position;
    }

    public void OnRightUngrab()
    {
        _isGrabbingRight = false;
    }
}