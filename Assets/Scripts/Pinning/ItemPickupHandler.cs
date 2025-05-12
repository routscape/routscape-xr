using System;
using System.Collections;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemPickupHandler: MonoBehaviour
{ 
    [SerializeField] private GameObject pinPlacementPrefab;
    [SerializeField] private Transform leftPinchArea;
    [SerializeField] private Transform rightPinchArea;
    [SerializeField] private HandGrabInteractor leftHandGrabInteractor;
    [SerializeField] private HandGrabInteractor rightHandGrabInteractor;

    private Vector3 _pinchArea;
    private HandGrabInteractor _handGrabInteractor;
    
    private void SpawnPin()
    {
        var instantiatedPin = Instantiate(pinPlacementPrefab, _pinchArea, quaternion.identity);
        var pinGrabbable = instantiatedPin.GetComponentInChildren<HandGrabInteractable>();
        StartCoroutine(SelectNextFrame(pinGrabbable));
    }
    
    private IEnumerator SelectNextFrame(HandGrabInteractable target)
    {
        yield return new WaitForEndOfFrame();

        _handGrabInteractor.ForceRelease(); 
        _handGrabInteractor.ForceSelect(target, true);
    }

    public void OnSelect(PointerEvent eventData)
    {
        Debug.Log("[ItemPickupHandler] OnClick");
        var gameObject = eventData.Data as GameObject;
        if (gameObject == null)
        {
            Debug.LogError("[ItemPickupHandler] Expected the gameobject (e.g., HandGrabInteractor) itself as a property of data within the interactor!");
            return;
        }

        if (gameObject.tag.Contains("left"))
        {
            _pinchArea = leftPinchArea.position;
            _handGrabInteractor = leftHandGrabInteractor;
        }
        else
        {
            _pinchArea = rightPinchArea.position;
            _handGrabInteractor = rightHandGrabInteractor;
        }

        SpawnPin();
    }
}
