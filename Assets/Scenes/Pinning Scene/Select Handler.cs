using System.Collections;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject _pinUI;

    private GameObject instantiatedPin = null;
    private HandGrabInteractable pinGrabbable = null;
    private bool _clicked = false;

    private RayInteractable _pinUIRayInteractable;
    private HandGrabInteractable _pinUIHandGrabInteractable;
    void Start()
    {
        _pinUIRayInteractable = _pinUI.GetComponent<RayInteractable>();
        _pinUIHandGrabInteractable = _pinUI.GetComponent<HandGrabInteractable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onGrabInteractorStateChanged(InteractorStateChangeArgs args)
    {
        Debug.Log("Grab old state " + args.PreviousState);
        Debug.Log("Grab New state " + args.NewState);

        if (args.NewState == InteractorState.Normal)
        {
            TogglePinUi();
            instantiatedPin.SetActive(true);
            _rightHandGrabInteractor.ForceSelect(pinGrabbable, true);
            _rightHandGrabInteractor.WhenStateChanged -= onGrabInteractorStateChanged;
            TogglePinUi();
        }
    }
    
    private IEnumerator SpawnPin()
    {
        instantiatedPin = Instantiate(_pinObject, _rightPinchArea.position, quaternion.identity);
        instantiatedPin.SetActive(false);

        pinGrabbable = instantiatedPin.GetComponentInChildren<HandGrabInteractable>();
        _rightRayInteractor.Disable();
        TogglePinUi();
        _rightHandGrabInteractor.ForceRelease();
        yield return new WaitUntil(() => _rightHandGrabInteractor.State == InteractorState.Normal);
        instantiatedPin.SetActive(true);
        _rightHandGrabInteractor.ForceSelect(pinGrabbable, true);;
        TogglePinUi();
    }

    public void TogglePinUi()
    {
        _pinUIRayInteractable.enabled = !_pinUIRayInteractable.enabled;
        _pinUIHandGrabInteractable.enabled = !_pinUIHandGrabInteractable.enabled;
    }
    
    public void OnClick(PointerEvent eventData)
    {

        Debug.Log(eventData.Type);
        Debug.Log(eventData.Data.GetType());
        HandRef handData = (HandRef)eventData.Data; 
        Debug.Log(handData.Handedness);
        StartCoroutine(SpawnPin());
    }
}
