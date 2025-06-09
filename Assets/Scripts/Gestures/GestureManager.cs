using System;
using Gestures;
using UnityEngine;

public class GestureManager : MonoBehaviour
{
    [SerializeField] private MapZoomHandler mapZoomHandler;
    public Action OnGestureEnd;

    private int _numSelections;

    void Start()
    {
        mapZoomHandler.OnZoomBegin += OnMapSelected;
        mapZoomHandler.OnZoomEnd += OnMapDeselected;
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
        OnGestureEnd?.Invoke(); 
    }
}
