using Oculus.Interaction;
using Oculus.Interaction.PoseDetection;
using UnityEngine;

public class GrabAndPinchHandler : MonoBehaviour
{
    [SerializeField] private RayInteractor leftRayInteractor;
    [SerializeField] private RayInteractor rightRayInteractor;
    [SerializeField] private FingerFeatureStateProvider leftHandFeatures;
    [SerializeField] private FingerFeatureStateProvider rightHandFeatures;

    public void OnSelect(PointerEvent eventData)
    {
    }
}