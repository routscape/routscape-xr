using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using UnityEngine;

namespace Clipping
{
    public class MapClipShaderModifier : MonoBehaviour
    {
        [SerializeField] private AbstractMap map;
        [SerializeField] private Shader newShader;

        private void OnEnable()
        {
            if (map == null) map = GetComponent<AbstractMap>();

            map.OnTileFinished += ModifyTileShader;
        }

        private void OnDestroy()
        {
            if (map != null)
                map.OnTileFinished -= ModifyTileShader;
        }

        private void ModifyTileShader(UnityTile tile)
        {
            if (tile == null || newShader == null) return;
            Debug.Log("[MapClipShaderModifier] Applying new shader to material of tile: " + tile.name);

            var meshRenderer = tile.MeshRenderer;

            var originalMat = meshRenderer.sharedMaterial;

            var newMat = new Material(originalMat);
            newMat.shader = newShader;

            if (newMat.HasProperty("_BaseMap"))
                newMat.SetTexture("_BaseMap", originalMat.GetTexture("_BaseMap"));

            meshRenderer.sharedMaterial = newMat;
        }
    }
}