using System;
using Mapbox.Unity.Map;
using Oculus.Interaction;
using UnityEngine;

public class PinDropper : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    private AbstractMap _mapManager;
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
        _mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out _hitInfo, 100f))
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, _hitInfo.point);
        }
        else
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
        }
    }

    public void OnDrop(PointerEvent eventData)
    {
        if (_hitInfo.point == Vector3.zero)
        {
            Destroy(transform.parent.parent.gameObject);
            return;
        }
        
        var pinName = SelectionService.NewMapObjectData.Name;
        var objectCategory = SelectionService.NewMapObjectData.ObjectCategory;
        var latlong = _mapManager.WorldToGeoPosition(_hitInfo.point);
        Debug.Log("[PinDropper] Pin name & pin type: " + pinName + ", " + objectCategory);
        _networkEventDispatcher.RPC_DropPin(pinName, latlong.x, latlong.y, (int)objectCategory); 
        Destroy(transform.parent.parent.gameObject);
    }
}