using UnityEngine;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.MeshGeneration.Modifiers;

[CreateAssetMenu(menuName = "Mapbox/Modifiers/PinModifier")]
public class PinModifier : GameObjectModifier
{
    // Public fields to store the user’s chosen color & text
    public Color pinColor = Color.white;
    public string pinText = "Default text";

    public override void Run(VectorEntity ve, UnityTile tile)
    {
        // This is the newly instantiated prefab/pin
        GameObject pinGO = ve.GameObject;

        Debug.Log("Object Modifier:" + pinGO.name);
        // if(renderer != null) renderer.material.color = pinColor;
    }
}

