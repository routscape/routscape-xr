using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class MapObjectCatalog : MonoBehaviour
{
    public static MapObjectCatalog I { get; private set; }
    public List<MapObjectType> mapObjectTypes;
    void Awake()
    {
        if (I && I != this) { DestroyImmediate(gameObject); return; }
        
        I = this;
    }

    public MapObjectType GetTypeInfo(MapObjectCategory objectCategory)
    {
        return mapObjectTypes.Find(mapObjectType => mapObjectType.objectCategory == objectCategory);
    }
}
