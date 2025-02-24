using Mapbox.Unity.Map;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using Oculus.Interaction.PoseDetection;
using UnityEngine;
using static Utils.HandGrabbing;

public class MapZoomHandler : MonoBehaviour
{
    [SerializeField] private AbstractMap mapManager;
    [SerializeField] private FingerFeatureStateProvider leftFingerFeatureStateProvider;
    [SerializeField] private FingerFeatureStateProvider rightFingerFeatureStateProvider;

    [SerializeField] private float zoomSpeed = 0.1f;
    private int _grabCount;

    private void LateUpdate()
    {
        if (_grabCount != 2) return;

        var leftHand = leftFingerFeatureStateProvider.Hand;
        var rightHand = rightFingerFeatureStateProvider.Hand;

        if (!IsGrabbing(leftHand) || !IsGrabbing(rightHand)) return;

        Pose leftHandPose, rightHandPose;
        leftHand.GetJointPose(HandJointId.HandPalm, out leftHandPose);
        rightHand.GetJointPose(HandJointId.HandPalm, out rightHandPose);
        var leftHandPosition = leftHandPose.position;
        var rightHandPosition = rightHandPose.position;

        var distance = Vector3.Distance(leftHandPosition, rightHandPosition);
        var zoomAmount = distance * zoomSpeed;

        var zoom = Mathf.Max(0.0f, Mathf.Min(mapManager.Zoom + zoomAmount, 21.0f));

        mapManager.UpdateMap(mapManager.CenterLatitudeLongitude, zoom);
    }

    public void OnSelect(PointerEvent pointerEvent)
    {
        var rayInteractor = pointerEvent.Data as RayInteractor;

        if (rayInteractor == null)
        {
            Debug.LogErrorFormat("[MapZoomHandler] Expected RayInteractor but got {0}",
                pointerEvent.Data.GetType().Name);
            return;
        }

        ++_grabCount;
    }

    public void OnDeselect(PointerEvent pointerEvent)
    {
        var rayInteractor = pointerEvent.Data as RayInteractor;

        if (rayInteractor == null)
        {
            Debug.LogErrorFormat("[MapZoomHandler] Expected RayInteractor but got {0}",
                pointerEvent.Data.GetType().Name);
            return;
        }

        --_grabCount;
    }
}