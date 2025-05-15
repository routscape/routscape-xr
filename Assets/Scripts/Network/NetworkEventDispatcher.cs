using System;
using Fusion;
using UnityEngine;

public class NetworkEventDispatcher : NetworkBehaviour
{
    public event Action<string, Vector3, int> OnPinDrop;
    public event Action<String, int> OnRouteBegin;
    public event Action OnRouteEnd;

    [Rpc]
    public void RPC_DropPin(string pinName, Vector3 hitInfo, int colorType)
    {
        OnPinDrop?.Invoke(pinName, hitInfo, colorType);
    }

    [Rpc]
    public void RPC_BeginRouteCreation(string routeName, int colorType)
    {
        OnRouteBegin?.Invoke(routeName, colorType);
        Debug.Log("[NetworkEventDispatcher] BeginRouteCreation");
    }
    [Rpc]
    public void RPC_EndRouteCreation()
    {
        OnRouteEnd?.Invoke();
        Debug.Log("[NetworkEventDispatcher] EndRouteCreation");

    }
}