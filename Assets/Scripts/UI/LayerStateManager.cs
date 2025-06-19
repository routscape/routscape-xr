using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class LayerState
{
    public bool State;
    public MapObjectCategory ObjectCategory;
}

public class LayerStateManager : MonoBehaviour
{
    public static LayerStateManager I { get; private set; }
    public IReadOnlyList<LayerState> LayerStates => _layerStates;           
    private List<LayerState> _layerStates = new List<LayerState>();
    public Action LayerStateChanged;
   
    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(this);
            return;
        }
        I = this;
    }

    void Start()
    {
        for (int i = 0; i < MapObjectCatalog.I.mapObjectTypes.Count; i++)
        {
            _layerStates.Add(new LayerState()
            {
                ObjectCategory = MapObjectCatalog.I.mapObjectTypes[i].objectCategory,
                State = true
            });
        }
    }

    public void SetLayerState(MapObjectCategory objectCategory, bool state)
    {
        _layerStates.Find(layer => layer.ObjectCategory == objectCategory).State = state;
        LayerStateChanged?.Invoke();
    }
}
