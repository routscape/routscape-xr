using System;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;

public class PencilProviderBehavior : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private GameObject pencilPrefab;
    private NetworkEventDispatcher _networkEventDispatcher;
    private Transform _pinchPoint;
    private GameObject _instantiatedPencil;
    void Start()
    {
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        if (_networkEventDispatcher== null)
        {
            Debug.Log("[PinDropper] No network event dispatcher found!");
            throw new Exception("[PinDropper] No network event dispatcher found!");
        }
        _pinchPoint = GameObject.FindWithTag("right pinch area").transform;
    }
    public void OnSelect(PointerEvent eventData)
    {
        _instantiatedPencil = Instantiate(pencilPrefab, _pinchPoint);
        _instantiatedPencil.transform.localPosition = new Vector3(0, -0.00630000001f, -0.00889999978f);
        meshRenderer.enabled = false;
        boxCollider.enabled = false;
    }
    
    public void OnRelease(PointerEvent eventData)
    {
        _instantiatedPencil.Destroy();
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
    }
}
