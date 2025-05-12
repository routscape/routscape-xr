using UnityEngine;
using System.Text.RegularExpressions;
using Mapbox.Utils;

public class Pin
{
    public string Name { get; private set; }
	public int ID { get; private set; }
    public Vector2d LatLong { get; private set; }
    public ColorType PinColorType { get; private set; }

    private GameObject _pinGameObject;

    public Color Color
    {
        get { return ColorHexCodes.GetColor(PinColorType); }
    }

    public Pin(GameObject pinGameObject, string name, Vector2d latLong, ColorType colorType)
    {
        _pinGameObject = pinGameObject;
        Name = name;
        ID = IDGenerator.GenerateID();
        PinColorType = colorType;
        LatLong = latLong;
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
        PinColorType = newColorType;
    }

    public void UpdateWorldPosition(Vector3 worldPosition)
    {
        _pinGameObject.transform.position = worldPosition;
    }

    public void UpdateWorldScale(float scale)
    {
        _pinGameObject.transform.localScale = new Vector3(scale, scale, scale);
    }

}
