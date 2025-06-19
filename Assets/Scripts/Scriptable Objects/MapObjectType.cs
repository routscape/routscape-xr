using UnityEngine;
using Utils;

[CreateAssetMenu(fileName = "MapObjectType", menuName = "Scriptable Objects/MapObjectType")]
public class MapObjectType : ScriptableObject
{
    public MapObjectCategory objectCategory;               
    public string displayName;
    public GameObject visualPrefab;
    public Sprite icon;
    public bool isSpawnableByPinJar;
}