using System;
using System.Collections.Generic;
using System.Linq;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Oculus.Interaction;
using Pins;
using UnityEngine;

public class PinRaycast : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject mapPin;
    private RaycastHit _hitInfo;
    private AbstractMap _mapManager;
    
    public static event Action<string, Vector2d, GameObject> OnPinDrop;
    public static HashSet<string> PinsDropped = new HashSet<string>();
    private static Vector2d currCoordinates;

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

        _pinSpawnHandler.MapPin = mapPin;
    }

    private void FixedUpdate()
    {
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out _hitInfo, 100f);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, _hitInfo.point);
    }

    public void OnDrop(PointerEvent eventData)
    {
        var latLong = _mapManager.WorldToGeoPosition(hitInfo.point);
        currCoordinates = latLong;
        string pinName = "Pin - " + currCoordinates.x + " " + currCoordinates.y;
        _mapManager.VectorData.SpawnPrefabAtGeoLocation(mapPin, latLong, PinDropCallback, scaleDownWithWorld: true, pinName);
        Destroy(transform.parent.parent.gameObject);
    }

    private void PinDropCallback(List<GameObject> items)
    {
        var pin = items.ElementAt(0);
        string pinName = "Pin - " + currCoordinates.x + " " + currCoordinates.y;
        Debug.Log("Pin ID " + pinName);
        if (PinsDropped.Contains(pinName))
        {
            return;
        }

        PinsDropped.Add(pinName);
        OnPinDrop.Invoke(pinName, currCoordinates, items.ElementAt(0));
    }
}