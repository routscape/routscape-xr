using System.Collections.Generic;
using UnityEngine;

public class MapObjectCatalog : MonoBehaviour
{
    public static MapObjectCatalog I { get; private set; }
    public List<MapObjectType> mapObjectTypes = new List<MapObjectType>();
    void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
    }
}
