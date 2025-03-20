using System;
using System.Collections;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class SelectHandler : MonoBehaviour
{ 
    [SerializeField] private GameObject _pinObject;
    [SerializeField] private Transform _leftPinchArea;
    [SerializeField] private Transform _rightPinchArea;
    [SerializeField] private HandGrabInteractor _leftHandGrabInteractor;
    [SerializeField] private HandGrabInteractor _rightHandGrabInteractor;
    [SerializeField] private RayInteractor _leftRayInteractor;
    [SerializeField] private RayInteractor _rightRayInteractor;
    [SerializeField] private GameObject _pinUI;

    private Vector3 _pinchArea;
    private HandGrabInteractor _handGrabInteractor;
    private RayInteractor _rayInteractor;
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
    
    private IEnumerator SpawnPin()
    {
        instantiatedPin = Instantiate(_pinObject, _pinchArea, quaternion.identity);
        instantiatedPin.SetActive(false);

        pinGrabbable = instantiatedPin.GetComponentInChildren<HandGrabInteractable>();
        _rayInteractor.Disable();
        TogglePinUi();
        _handGrabInteractor.ForceRelease();
        yield return new WaitUntil(() => _handGrabInteractor.State == InteractorState.Normal);
        instantiatedPin.SetActive(true);
        _handGrabInteractor.ForceSelect(pinGrabbable, true);;
        TogglePinUi();
    }

    public void TogglePinUi()
    {
        _pinUIRayInteractable.enabled = !_pinUIRayInteractable.enabled;
        _pinUIHandGrabInteractable.enabled = !_pinUIHandGrabInteractable.enabled;
    }
    
    public void OnClick(PointerEvent eventData)
    {
        var gameObject = eventData.Data as GameObject;
        if (gameObject == null)
        {
            Debug.LogError("[FLOOD CUBE] interactor expected in Data property of hand!");
            return;
        }

        if (gameObject.tag.Contains("left"))
        {
            _pinchArea = _leftPinchArea.position;
            _handGrabInteractor = _leftHandGrabInteractor;
            _rayInteractor = _leftRayInteractor;
        }
        else
        {
            _pinchArea = _rightPinchArea.position;
            _handGrabInteractor = _rightHandGrabInteractor;
            _rayInteractor = _leftRayInteractor;
        }
        
        StartCoroutine(SpawnPin());
    }
}
