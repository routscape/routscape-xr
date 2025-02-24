using UnityEngine;
using System.Collections.Generic;

public class Route
{
    public string Name { get; private set; }
    public Color LineColor { get; set; }
    private LineRenderer lineRenderer;
    private List<Vector3> routePoints;

    public Route(string name, LineRenderer renderer, Color color)
    {
        Name = name;
        LineColor = color;
        lineRenderer = renderer;
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
}