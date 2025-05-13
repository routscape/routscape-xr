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
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform leftPinchArea;
    [SerializeField] private Transform rightPinchArea;
    [SerializeField] private HandGrabInteractor leftHandGrabInteractor;
    [SerializeField] private HandGrabInteractor rightHandGrabInteractor;

    private Vector3 _pinchArea;
    private HandGrabInteractor _handGrabInteractor;
    
    private void SpawnItem()
    {
        var instantiatedObject= Instantiate(itemPrefab, _pinchArea, quaternion.identity);
        var objectGrabbable = instantiatedObject.GetComponentInChildren<HandGrabInteractable>();
        StartCoroutine(SelectNextFrame(objectGrabbable));
    }
    
    private IEnumerator SelectNextFrame(HandGrabInteractable target)
    {
        yield return new WaitForEndOfFrame();

        _handGrabInteractor.ForceRelease(); 
        _handGrabInteractor.ForceSelect(target, true);
    }

    public void OnSelect(PointerEvent eventData)
    {
        var interactorGameObject = eventData.Data as GameObject;
        if (interactorGameObject == null)
        {
            Debug.LogError("[ItemPickupHandler] Expected the gameobject (e.g., HandGrabInteractor) itself as a property of data within the interactor!");
            return;
        }

        if (interactorGameObject.tag.Contains("left"))
        {
            _pinchArea = leftPinchArea.position;
            _handGrabInteractor = leftHandGrabInteractor;
        }
        else
        {
            _pinchArea = rightPinchArea.position;
            _handGrabInteractor = rightHandGrabInteractor;
        }

        SpawnItem();
    }
}
