using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;

public class Route
{
    public string Name { get; private set; }
    public string Id { get; private set; }

    public GameObject prefab;
    public ColorType RouteColorType { get; private set; }
    public Color Color => ColorHexCodes.GetColor(RouteColorType);
    public List<Vector2d> routePointsMap { get; }
    public LineRenderer lineRenderer;
    private List<Vector3> routePoints;

    public Route(string name, string id, LineRenderer renderer, ColorType colorType)
    {
        Name = name;
        Id = id;
        lineRenderer = renderer;
		RouteColorType = colorType;
        routePoints = new List<Vector3>();
        routePointsMap = new List<Vector2d>();
    }

    public void AddPoint(Vector3 point, AbstractMap mapManager)
    {
        routePoints.Add(point);
        routePointsMap.Add(mapManager.WorldToGeoPosition(point));
        lineRenderer.positionCount = routePoints.Count;
        lineRenderer.SetPosition(routePoints.Count - 1, point);
    }

    public Vector2d GetLocation()
    {
        if (routePointsMap.Count == 0)
        {
            return new Vector2d(0f, 0f);
        }

        return routePointsMap.ElementAt(0);
    }

    public int GetPointCount()
    {
        return routePoints.Count;
    }

    public Vector3 GetRouteStartPoint()
    {
        return routePoints.ElementAt(0);
    }
    
    public void Rename(string newName)
    {
        string cleanedName = Regex.Replace(newName, @"\p{C}+", "").Trim();
        if (!string.IsNullOrWhiteSpace(cleanedName))  // Prevents empty names
        {
            Name = newName;
        }
    }
    
    public void ChangeColor(ColorType newColorType)
    {
        RouteColorType = newColorType;
    }
}