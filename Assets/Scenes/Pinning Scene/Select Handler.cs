using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using Unity.Mathematics;
using UnityEngine;

public class SelectHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject _pinObject;
    [SerializeField] private Transform _leftPinchArea;
    [SerializeField] private Transform _rightPinchArea;
    [SerializeField] private HandGrabInteractor _leftHandGrabInteractor;
    [SerializeField] private HandGrabInteractor _rightHandGrabInteractor;
    [SerializeField] private RayInteractor _leftRayInteractor;
    [SerializeField] private RayInteractor _rightRayInteractor;
    private bool _clicked = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnPin()
    {
        var instantiatedPin = Instantiate(_pinObject, _rightPinchArea.position, quaternion.identity);
        instantiatedPin.SetActive(false);
        Debug.Log("Interactor state: " + _rightHandGrabInteractor.State);
        HandGrabInteractable pinGrabbable = instantiatedPin.GetComponentInChildren<HandGrabInteractable>();
        if (pinGrabbable == null)
        {
            Debug.Log("Hand grab interactable component missing!");
            return;
        }
        /*
         * Problem: Interactor is disabled when I select the button for some odd reason
         *
         * Investigate if other interactors can affect the state of other interactors.
         */
       
        _rightRayInteractor.Disable();
        _rightHandGrabInteractor.Enable();
        instantiatedPin.SetActive(true);
        _rightHandGrabInteractor.ForceSelect(pinGrabbable, true);
        
        Debug.Log("Pin state: " + pinGrabbable.State);
    }

    public void OnClick(PointerEvent eventData)
    {
        if (_rightRayInteractor.State == InteractorState.Disabled)
        {
            return;
        }
        if (_clicked)
        {
            _clicked = false;
            return;
        }
        _clicked = true;
        HandRef handData = (HandRef)eventData.Data;
        Debug.Log(handData.Handedness);
        SpawnPin();
    }
}
