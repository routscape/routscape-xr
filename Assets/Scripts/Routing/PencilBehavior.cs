using System;
using Oculus.Interaction;
using UnityEngine;

public class PencilBehavior : MonoBehaviour
{
    private NetworkEventDispatcher _networkEventDispatcher;
    void Start()
    {
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        if (_networkEventDispatcher== null)
        {
            Debug.Log("[PinDropper] No network event dispatcher found!");
            throw new Exception("[PinDropper] No network event dispatcher found!");
        }
    }
    public void OnSelect(PointerEvent eventData)
    {
        _networkEventDispatcher.RPC_BeginRouteCreation("Route 1", 0);
    }
    public void OnDeselect(PointerEvent eventData)
    {
        _networkEventDispatcher.RPC_EndRouteCreation();
    }
}
