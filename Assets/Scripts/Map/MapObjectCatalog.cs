using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapObjectCatalog : MonoBehaviour
{
    public static MapObjectCatalog I { get; private set; }
    public List<MapObjectType> mapObjectTypes;
    void Awake()
    {
        if (I && I != this) { DestroyImmediate(gameObject); return; }
        
        I = this;
    }
}
