using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Route
{
    public string Name { get; private set; }
    public ColorType RouteColorType { get; private set; }
    public Color Color => ColorHexCodes.GetColor(RouteColorType);
    private LineRenderer lineRenderer;
    private List<Vector3> routePoints;

    public Route(string name, LineRenderer renderer, ColorType colorType)
    {
        Name = name;
        lineRenderer = renderer;
		RouteColorType = colorType;
        routePoints = new List<Vector3>();
    }

    public void AddPoint(Vector3 point)
    {
        routePoints.Add(point);
        lineRenderer.positionCount = routePoints.Count;
        lineRenderer.SetPosition(routePoints.Count - 1, point);
    }

    public int GetPointCount()
    {
        return routePoints.Count;
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