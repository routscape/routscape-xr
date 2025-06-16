using System;
using Gestures;
using UnityEngine;

public class GestureManager : MonoBehaviour
{
    [SerializeField] private MapZoomHandler mapZoomHandler;
    private NetworkEventDispatcher _networkEventDispatcher;

    private int _numSelections;

    void Start()
    {
        _networkEventDispatcher =
            GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        _networkEventDispatcher.OnZoomBegin += OnMapSelected;
        _networkEventDispatcher.OnZoomEnd += OnMapDeselected;
    }
    
    public void OnMapSelected()
    {
        _numSelections++;
    }

    public void OnMapDeselected()
    {
        _numSelections--;
        if (mapZoomHandler.IsZooming || _numSelections != 0)
        {
            return;
        }
        Debug.Log("[GestureManager] on gesture end");
        _networkEventDispatcher.RPC_GestureEnd();
    }
}
