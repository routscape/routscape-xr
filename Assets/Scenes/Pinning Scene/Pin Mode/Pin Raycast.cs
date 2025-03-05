using System;
using System.Linq;
using Mapbox.Unity.Map;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class PinRaycast : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject mapPin;
    private RaycastHit hitInfo;
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

    void FixedUpdate()
    {
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hitInfo, 100f);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hitInfo.point);
    }

    public void OnDrop(PointerEvent eventData)
    {
        var latLong = _mapManager.WorldToGeoPosition(hitInfo.point);
        _mapManager.VectorData.SpawnPrefabAtGeoLocation(mapPin, latLong, callback: null, scaleDownWithWorld: true, "Pin");
        Debug.Log(_mapManager.VectorData.GetPointsOfInterestSubLayerAtIndex(0).coordinates[0]);
        Destroy(transform.parent.parent.gameObject);
    }
}
