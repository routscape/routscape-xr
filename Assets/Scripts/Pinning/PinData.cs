using System;
using UnityEngine;
using System.Text.RegularExpressions;
using Mapbox.Utils;
using Utils;

public class PinData
{
    public string Name { get; private set; }
	public int ID { get; private set; }
    public MapObjectCategory ObjectCategory { get; private set; }
    public Vector2d LatLong { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public float WorldScale { get; private set; }
    public ColorType PinColorType { get; private set; }
    public Color Color
    {
        get { return ColorHexCodes.GetColor(PinColorType); }
    }

    public Action<PinData> OnPinDataChanged;
    public PinData(string name, Vector3 worldPosition, MapObjectCategory objectCategory)
    {
        Name = name;
        WorldPosition = worldPosition;
        ID = IDGenerator.GenerateID();
        ObjectCategory = objectCategory;
    }

    public void ChangeName(string newName)
    {
        string cleanedName = Regex.Replace(newName, @"\p{C}+", "").Trim();
        if (!string.IsNullOrWhiteSpace(cleanedName))  
        {
            Name = newName;
        }
        OnPinDataChanged?.Invoke(this);
    }
    
    public void ChangeColor(ColorType newColorType)
    {
        PinColorType = newColorType;
        OnPinDataChanged?.Invoke(this);
    }

    public void ChangeLatLong(Vector2d latLong)
    {
        LatLong = latLong;
        OnPinDataChanged?.Invoke(this);
    }
    
    public void UpdateWorldPosition(Vector3 position)
    {
        WorldPosition = position;
        OnPinDataChanged?.Invoke(this);
    }

    public void UpdateWorldScale(float scale)
    {
        WorldScale = scale;
        OnPinDataChanged.Invoke(this);
    }
}
