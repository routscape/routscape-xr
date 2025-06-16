using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;

public class MeshBehavior : MonoBehaviour
{
    [SerializeField] private AbstractMap _mapManager;
    private NetworkEventDispatcher _networkEventDispatcher;

    private double _heightConstant;
    
    void Start()
    {
        _networkEventDispatcher = GameObject.FindGameObjectWithTag("network event dispatcher")
            .GetComponent<NetworkEventDispatcher>();

        InitializeHeightConstant();
        _networkEventDispatcher.OnZoomEnd += RecalculateHeight;
    }

    void InitializeHeightConstant()
    {
        var unitsPerMeter = GetUnitsPerMeterScaled();
        _heightConstant = transform.localPosition.y;
        _heightConstant /= unitsPerMeter;
    }

    void RecalculateHeight()
    {
        double unitsPerMeter = GetUnitsPerMeterScaled();
        transform.localPosition = new Vector3(transform.localPosition.x, (float)(_heightConstant * unitsPerMeter),
            transform.localPosition.z);
    }
    
    private double GetUnitsPerMeterScaled()
    {
        var referenceTileRect =
            Conversions.TileBounds(TileCover.CoordinateToTileId(_mapManager.CenterLatitudeLongitude,
                _mapManager.AbsoluteZoom));
        double zoomDifference = _mapManager.Zoom - _mapManager.AbsoluteZoom;
        var unitsPerMeter = _mapManager.Options.scalingOptions.unityTileSize / referenceTileRect.Size.x * 
                            Mathd.Pow(2d, zoomDifference);
        return unitsPerMeter;
    }
}
