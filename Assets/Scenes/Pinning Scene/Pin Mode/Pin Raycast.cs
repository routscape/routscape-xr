using System;
using Fusion;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Oculus.Interaction;
using UnityEngine;

public class PinRaycast : NetworkBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject mapPin;
    private RaycastHit _hitInfo;
    private AbstractMap _mapManager;

    private void Start()
    {
        _mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
        if (_mapManager == null)
        {
            Debug.Log("Pin: No map found!");
            throw new Exception("Pin: No map found!");
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
        RpcSpawnPin(latLong.ToVector3xz());
        Destroy(transform.parent.parent.gameObject);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcSpawnPin(Vector3 position)
    {
        _mapManager.VectorData.SpawnPrefabAtGeoLocation(mapPin, position.ToVector2d(), null, true, "Pin");
    }
}