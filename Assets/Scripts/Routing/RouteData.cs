using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Utils;

public class RouteData 
{
    public string Name { get; private set; }
    public int ID { get; private set; }
    public Transform ParentTransform { get; private set; }
    public Vector2d ParentLatLong { get; private set; }
    public MapObjectCategory ObjectCategory { get; private set; }
    public ColorType RouteColorType { get; private set; }
    public Color Color => ColorHexCodes.GetColor(RouteColorType);
    public Action<RouteData> OnRouteDataChanged;
    public Action<Vector3> OnRoutePointAdded;
    public Action OnRouteBakeMesh;
    public Action<int, Vector3> OnRoutePointModified;
    public Action OnDelete;
    public List<Vector2d> RoutePointsLatLong { get; private set; }
    
    public RouteData(string name, MapObjectCategory objectCategory, ColorType colorType)
    {
        Name = name;
        ID = IDGenerator.GenerateID();
        ObjectCategory = objectCategory;
		RouteColorType = colorType;
        RoutePointsLatLong = new List<Vector2d>();
    }

    public void AddPoint(Vector2d point, Vector3 worldPoint)
    {
        RoutePointsLatLong.Add(point);
        OnRoutePointAdded?.Invoke(worldPoint);
    }

    public void SetVertexPosition(int index, Vector3 position)
    {
        position.y += 6f;
        OnRoutePointModified?.Invoke(index, position);
    }

    public void SetParentTransform(Transform transform)
    {
        ParentTransform = transform;
    }
    
    public Vector2d GetLocation()
    {
        if (RoutePointsLatLong.Count == 0)
        {
            return new Vector2d(0f, 0f);
        }

        return RoutePointsLatLong.ElementAt(0);
    }

    public void SetParentLatLong(Vector2d latLong)
    {
        ParentLatLong = latLong;
    }

    public void UpdateWorldPosition(Vector3 worldPosition)
    {
        ParentTransform.position = worldPosition;
        //hacky solution, data class directly influences behavior without event, idk any other workarounds
    }

    public void BakeMesh()
    {
        OnRouteBakeMesh?.Invoke();
    }

    public int GetPointCount()
    {
        return RoutePointsLatLong.Count;
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

    public void DeleteSelf()
    {
        OnDelete?.Invoke();
    }
}