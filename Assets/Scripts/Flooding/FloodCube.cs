using Mapbox.Unity.Map;
using TMPro;
using UnityEngine;

namespace Flooding
{
    public class FloodCube : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
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

        public void SetText(string text)
        {
            label.text = text;
        }

        public float GetBoundsY()
        {
            return _renderer.bounds.max.y;
        }
    }
}