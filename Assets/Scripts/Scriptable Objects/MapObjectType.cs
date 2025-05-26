using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "MapObjectType", menuName = "Scriptable Objects/MapObjectType")]
public class MapObjectType : ScriptableObject
{
    [Tooltip("Unique, stable numeric ID you’ll send over the wire")]
    public int typeID;               

    [Tooltip("Default object label for UI")]
    public string displayName;      

    [Tooltip("Prefab to spawn for this type")]
    public GameObject prefab;

    [Tooltip("Image for display on the UI")]
    public Sprite icon;
}
