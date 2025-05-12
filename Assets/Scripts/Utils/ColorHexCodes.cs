using UnityEngine;

public enum ColorType
{
    Red = 0,
    Green = 1,
    Blue = 2
}

public static class ColorHexCodes
{
    public const string Red = "#E04F4F";
    public const string Green = "#4FE048";
    public const string Blue = "#4FABE0";

    public static Color GetColor(ColorType colorType)
    {
        return colorType switch
        {
            ColorType.Red => HexToColor(Red),
            ColorType.Green => HexToColor(Green),
            ColorType.Blue => HexToColor(Blue),
            _ => Color.white
        };
    }

    private static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        return Color.white; // Default color
    }
}