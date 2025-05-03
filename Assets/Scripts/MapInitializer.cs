using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class MapInitializer : MonoBehaviour
{
    [SerializeField] private AbstractMap map;
    [SerializeField] private Vector2d startCoordinates;
    [SerializeField] private int startZoom;

    public void InitializeMap()
    {
        if (map == null) map = GetComponent<AbstractMap>();

        map.Initialize(startCoordinates, startZoom);
    }
}