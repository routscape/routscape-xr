using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;

public class RouteData
{
    public string Name { get; private set; }
    public int ID { get; private set; }
    public ColorType RouteColorType { get; private set; }
    public Color Color => ColorHexCodes.GetColor(RouteColorType);
    public Action<RouteData> OnRouteDataChanged;
    public Action<Vector3> OnRoutePointAdded;
    public List<Vector2d> routePointsLatLong { get; private set; }
    
    public RouteData(string name, ColorType colorType)
    {
        Name = name;
        ID = IDGenerator.GenerateID();
		RouteColorType = colorType;
        routePointsLatLong = new List<Vector2d>();
    }

    public void AddPoint(Vector2d point, Vector3 worldPoint)
    {
        routePointsLatLong.Add(point);
        OnRoutePointAdded?.Invoke(worldPoint);
    }

    public Vector2d GetLocation()
    {
        if (routePointsLatLong.Count == 0)
        {
            return new Vector2d(0f, 0f);
        }

        return routePointsLatLong.ElementAt(0);
    }

    public int GetPointCount()
    {
        return routePointsLatLong.Count;
    }
    
    
    public void Rename(string newName)
    {
        string cleanedName = Regex.Replace(newName, @"\p{C}+", "").Trim();
        if (!string.IsNullOrWhiteSpace(cleanedName))  // Prevents empty names
        {
            Name = newName;
        }
        OnRouteDataChanged?.Invoke(this);
    }
    
    public void ChangeColor(ColorType newColorType)
    {
        RouteColorType = newColorType;
        OnRouteDataChanged?.Invoke(this);
    }
}