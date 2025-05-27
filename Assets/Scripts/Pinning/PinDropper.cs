using System;
using Oculus.Interaction;
using UnityEngine;

public class PinDropper : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    
    private RaycastHit _hitInfo;
    private NetworkEventDispatcher _networkEventDispatcher;
    private void Start()
    {
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        if (_networkEventDispatcher== null)
        {
            Debug.Log("[PinDropper] No network event dispatcher found!");
            throw new Exception("[PinDropper] No network event dispatcher found!");
        }
    }

    private void FixedUpdate()
    {
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out _hitInfo, 100f);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, _hitInfo.point);
    }

    public void OnDrop(PointerEvent eventData)
    {
        var pinName = SelectionService.NewMapObjectData.Name;
        var objectCategory = SelectionService.NewMapObjectData.ObjectCategory;
        _networkEventDispatcher.RPC_DropPin(pinName, _hitInfo.point, (int)objectCategory); 
        Destroy(transform.parent.parent.gameObject);
    }
}