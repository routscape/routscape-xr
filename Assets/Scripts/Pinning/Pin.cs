using UnityEngine;
using System.Text.RegularExpressions;

public class Pin
{
    public string Name { get; private set; }
	// public string Barangay { get; private set; } // temp
    // public Vector3 Position { get; private set; }
	public string MapboxPinId { get; private set; }
    public ColorType PinColorType { get; private set; }
    public Color Color => ColorHexCodes.GetColor(PinColorType);

    public Pin(string name, string mapboxId , ColorType colorType)
    {
        Name = name;
		MapboxPinId = mapboxId;
        PinColorType = colorType;
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

}
