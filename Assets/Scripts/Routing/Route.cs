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

    public void ApplySmoothing(int iterations = 2)
    {
        var smoothedPoints = SmoothLine(routePoints, iterations);
        lineRenderer.positionCount = smoothedPoints.Count;
        lineRenderer.SetPositions(smoothedPoints.ToArray());
    }

    private List<Vector3> SmoothLine(List<Vector3> inputPoints, int iterations = 2)
    {
        List<Vector3> points = new List<Vector3>(inputPoints);

        for (int iter = 0; iter < iterations; iter++)
        {
            List<Vector3> newPoints = new List<Vector3>();

            if (points.Count < 2)
                return points;

            newPoints.Add(points[0]); // Preserve the first point

            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector3 p0 = points[i];
                Vector3 p1 = points[i + 1];

                Vector3 Q = Vector3.Lerp(p0, p1, 0.25f); // 25% between
                Vector3 R = Vector3.Lerp(p0, p1, 0.75f); // 75% between

                newPoints.Add(Q);
                newPoints.Add(R);
            }

            newPoints.Add(points[points.Count - 1]); // Preserve the last point

            points = newPoints;
        }

        return points;
    }

}