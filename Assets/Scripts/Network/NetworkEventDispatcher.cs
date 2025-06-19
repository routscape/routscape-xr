using System;
using Fusion;
using UnityEngine;

public class NetworkEventDispatcher : NetworkBehaviour
{
    public event Action<string, double, double, int> OnPinDrop;
    public event Action<String, int, int> OnRouteBegin;
    public event Action OnRouteEnd;
    public event Action<int, string> OnHighlightListItem;
    public event Action<int> OnJumpToMapObject;
    public event Action<int> OnEraseMapObject;
    public event Action OnEraseBegin; //select the eraser
    public event Action OnEraseEnd; //release the eraser
    public event Action<int, double, double> OnRepositionPin;
    public event Action OnToggleFlood;
    public event Action OnZoomBegin;
    public event Action OnZoomEnd;
    public event Action OnGestureEnd;
    [Rpc]
    public void RPC_DropPin(string pinName, double latitude, double longitude, int objectCategory)
    {
        OnPinDrop?.Invoke(pinName, latitude, longitude, objectCategory);
    }

    [Rpc]
    public void RPC_BeginRouteCreation(string routeName, int objectCategory, int colorType)
    {
        OnRouteBegin?.Invoke(routeName, objectCategory, colorType);
        Debug.Log("[NetworkEventDispatcher] BeginRouteCreation");
    }
    
    [Rpc]
    public void RPC_EndRouteCreation()
    {
        OnRouteEnd?.Invoke();
        Debug.Log("[NetworkEventDispatcher] EndRouteCreation");

    }

    [Rpc]
    public void RPC_HighlightListItem(int objectID, string listColorType)
    {
       OnHighlightListItem?.Invoke(objectID, listColorType); 
       Debug.Log("[NetworkEventDispatcher] HighlightListItem");
    }

    [Rpc]
    public void RPC_JumpToMapObject(int objectID)
    {
        OnJumpToMapObject?.Invoke(objectID);
        Debug.Log("[NetworkEventDispatcher] JumpToMapObject");
    }

    [Rpc]
    public void RPC_EraseMapObject(int objectID)
    {
        Debug.Log("[NetworkEventDispatcher] EraseMapObject");
        OnEraseMapObject?.Invoke(objectID);
    }
    
    [Rpc]
    public void RPC_EraseBegin()
    {
        Debug.Log("[NetworkEventDispatcher] EraseBegin");
        OnEraseBegin?.Invoke();
    }
    
    [Rpc]
    public void RPC_EraseEnd()
    {
        Debug.Log("[NetworkEventDispatcher] EraseEnd");
        OnEraseEnd?.Invoke();
    }

    [Rpc]
    public void RPC_RepositionPin(int objectID, double latitude, double longitude)
    {
        OnRepositionPin?.Invoke(objectID, latitude, longitude);
    }

    [Rpc]
    public void RPC_ToggleFlood()
    {
        OnToggleFlood?.Invoke();
    }

    [Rpc]
    public void RPC_ZoomBegin()
    {
        OnZoomBegin?.Invoke();
    }
    
    [Rpc]
    public void RPC_ZoomEnd()
    {
        OnZoomEnd?.Invoke();
    }

    [Rpc]
    public void RPC_GestureEnd()
    {
        OnGestureEnd?.Invoke();
    }
}