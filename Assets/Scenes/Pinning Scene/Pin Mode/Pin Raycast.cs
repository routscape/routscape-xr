using System;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Oculus.Interaction;
using Pinning;
using UnityEngine;

public class PinRaycast : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject mapPin;
    private RaycastHit _hitInfo;
    private AbstractMap _mapManager;
    private PersistentPinSpawnHandler _pinSpawnHandler;

    private void Start()
    {
        _mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
        if (_mapManager == null)
        {
            Debug.Log("Pin: No map found!");
            throw new Exception("Pin: No map found!");
        }

        _pinSpawnHandler = GameObject.FindWithTag("network persistence").GetComponent<PersistentPinSpawnHandler>();
        if (_pinSpawnHandler == null)
        {
            Debug.Log("Pin: No network persistence found!");
            throw new Exception("Pin: No network persistence found!");
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
        var latLong = _mapManager.WorldToGeoPosition(_hitInfo.point);
        _pinSpawnHandler.RpcSpawnPin(latLong.ToVector3xz());
        Destroy(transform.parent.parent.gameObject);
    }
}