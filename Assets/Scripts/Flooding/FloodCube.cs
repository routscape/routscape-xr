using Mapbox.Unity.Map;
using TMPro;
using UnityEngine;

namespace Flooding
{
    public class FloodCube : MonoBehaviour
    {
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");

        private MaterialPropertyBlock _propertyBlock;
        private Renderer _renderer;

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
            _renderer = GetComponent<Renderer>();
        }

        public void SetColor(Color color)
        {
            _propertyBlock.SetColor(ColorProperty, color);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}