using System;
using Fusion;

public class NetworkEventDispatcher : NetworkBehaviour
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