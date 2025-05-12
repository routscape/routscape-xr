using System;
using Fusion;
using Mapbox.Utils;
using UnityEngine;

public class NetworkEventDispatcherSingleton : NetworkBehaviour
{
    private static NetworkEventDispatcher _networkEventDispatcher;

    public static NetworkEventDispatcher GetInstance()
    {
        if (_networkEventDispatcher == null)
        { 
            _networkEventDispatcher = new NetworkEventDispatcher();
        }

        return _networkEventDispatcher;
    }
}

public class NetworkEventDispatcher
{
    public static event Action<string, double, double, int> OnPinDrop;
    public static event Action OnRouteBegin;
    
    [Rpc]
    public void RPC_DropPin(string pinName, double x, double y, int colorType)
    {
        OnPinDrop?.Invoke(pinName, x, y, colorType);
    }

    [Rpc]
    public void RPC_BeginRouteCreation()
    {
        OnRouteBegin?.Invoke(); 
    }
    
}