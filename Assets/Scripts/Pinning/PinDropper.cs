using System;
using Oculus.Interaction;
using UnityEngine;

public class PinDropper : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject mapPin;
    
    private RaycastHit _hitInfo;
    private NetworkEventDispatcher _networkEventDispatcher;
    private void Start()
    {
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        if (_networkEventDispatcher== null)
        {
            Debug.Log("Pin: No network persistence found!");
            throw new Exception("[PinDropper] No network persistence found!");
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
        _networkEventDispatcher.RPC_DropPin("Pin", _hitInfo.point, (int)ColorType.Red);
        Destroy(transform.parent.parent.gameObject);
    }
}