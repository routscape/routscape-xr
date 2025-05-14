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
    public GameObject itemPrefab;
    private Transform _leftPinchArea;
    private Transform _rightPinchArea;
    private HandGrabInteractor _leftHandGrabInteractor;
    private HandGrabInteractor _rightHandGrabInteractor;
    
    public Action<GameObject> OnInstantiateObject;
    
    private Transform _pinchArea;
    private HandGrabInteractor _handGrabInteractor;
    void Start()
    {
        if (itemPrefab == null)
        {
            Debug.LogError("[ItemPickupHandler] Missing item prefab!");
        }

        _leftPinchArea = GameObject.FindWithTag("left pinch area").GetComponent<Transform>();
        if (_leftPinchArea == null)
        {
            Debug.LogError("[ItemPickupHandler] Missing left pinch area!");
        }
        _rightPinchArea = GameObject.FindWithTag("right pinch area").GetComponent<Transform>();
        if (_rightPinchArea== null)
        {
            Debug.LogError("[ItemPickupHandler] Missing right pinch area!");
        }
        _leftHandGrabInteractor = GameObject.FindWithTag("left hand grab interactor").GetComponent<HandGrabInteractor>();
        if (_leftHandGrabInteractor == null)
        {
            Debug.LogError("[ItemPickupHandler] Missing left hand grab interactor!");
        }
        _rightHandGrabInteractor = GameObject.FindWithTag("right hand grab interactor").GetComponent<HandGrabInteractor>();
        if (_rightHandGrabInteractor == null)
        {
            Debug.LogError("[ItemPickupHandler] Missing right hand grab interactor!");
        }
    }
    public void SetItemPrefab(GameObject go)
    {
        itemPrefab = go;
    }
    private void SpawnItem()
    {
        var instantiatedObject= Instantiate(itemPrefab, _pinchArea.position, quaternion.identity);
        var objectGrabbable = instantiatedObject.GetComponentInChildren<HandGrabInteractable>();
        StartCoroutine(SelectNextFrame(objectGrabbable));
        Debug.Log("[ItemPickupHandler] " + instantiatedObject);
        OnInstantiateObject?.Invoke(instantiatedObject);
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
            _pinchArea = _leftPinchArea;
            _handGrabInteractor = _leftHandGrabInteractor;
        }
        else
        {
            _pinchArea = _rightPinchArea;
            _handGrabInteractor = _rightHandGrabInteractor;
        }

        SpawnItem();
    }
}
