using Oculus.Interaction;
using Oculus.Interaction.Input;
using Oculus.Interaction.PoseDetection;
using UnityEngine;

public class ConstrainRayInteractionToPinch : MonoBehaviour
{
    [SerializeField] private RayInteractor leftRayInteractor;
    [SerializeField] private RayInteractor rightRayInteractor;
    [SerializeField] private FingerFeatureStateProvider leftHandFeatures;
    [SerializeField] private FingerFeatureStateProvider rightHandFeatures;

    private bool IsGrabbing(IHand hand)
    {
        return hand.GetFingerIsPinching(HandFinger.Index) &&
               hand.GetFingerIsPinching(HandFinger.Middle) &&
               hand.GetFingerIsPinching(HandFinger.Ring) &&
               hand.GetFingerIsPinching(HandFinger.Pinky);
    }

    public void OnSelect(PointerEvent eventData)
    {
        if (!leftHandFeatures.Hand.GetFingerIsPinching(HandFinger.Index) &&
            !rightHandFeatures.Hand.GetFingerIsPinching(HandFinger.Index))
        {
        }
    }
}