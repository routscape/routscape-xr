using System;
using Fusion;
using UnityEngine;

public class NetworkEventDispatcher : NetworkBehaviour
{
    public static event Action<string, Vector3, int> OnPinDrop;
    public static event Action OnRouteBegin;

    [Rpc]
    public void RPC_DropPin(string pinName, Vector3 hitInfo, int colorType)
    {
        OnPinDrop?.Invoke(pinName, hitInfo, colorType);
    }

    [Rpc]
    public void RPC_BeginRouteCreation()
    {
        OnRouteBegin?.Invoke();
    }
}