using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using UnityEngine;

namespace Clipping
{
    [ExecuteAlways]
    public class MapClipShaderModifier : MonoBehaviour
    {
        [SerializeField] private AbstractMap map;
        [SerializeField] private Shader newShader;

        [Header("Shader Properties")] [SerializeField]
        private Vector4 clipRegion = Vector4.zero;

        private readonly Dictionary<string, Material> _materialCache = new();

        private void Start()
        {
            if (map == null) map = GetComponent<AbstractMap>();

            map.OnTileFinished += OnTileFinished;
            map.OnEditorPreviewDisabled += ClearCache;
        }

        private void OnDestroy()
        {
            if (map != null)
                map.OnTileFinished -= OnTileFinished;

            ClearCache();
        }

        private void ClearCache()
        {
            foreach (var material in _materialCache.Values)
                if (material != null)
                    DestroyImmediate(material);
            _materialCache.Clear();
        }

        private void OnTileFinished(UnityTile tile)
        {
            if (tile == null || newShader == null) return;
            Debug.Log("[MapClipShaderModifier] Applying new shader to material of tile: " + tile.name);

            var meshRenderer = tile.MeshRenderer;

            var originalMat = meshRenderer.sharedMaterial;

            var cacheKey = tile.name;
            if (!_materialCache.TryGetValue(cacheKey, out var newMat))
            {
                newMat = new Material(originalMat);
                newMat.shader = newShader;
                _materialCache[cacheKey] = newMat;
            }

            if (newMat.HasProperty("_ClipRegion"))
                newMat.SetVector("_ClipRegion", clipRegion);

            if (newMat.HasProperty("_BaseMap"))
                newMat.SetTexture("_BaseMap", originalMat.GetTexture("_BaseMap"));

            meshRenderer.sharedMaterial = newMat;
        }
    }
}