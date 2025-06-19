using UnityEngine;

namespace Clipping
{
    public class ClipRegionController : MonoBehaviour
    {
        private Vector4 _clipRegion;

        private void OnEnable()
        {
            Shader.SetGlobalVector("_ClipRegion", _clipRegion);
        }

        public void SetClipRegion(Vector4 newClipRegion)
        {
            _clipRegion = newClipRegion;

            Debug.Log($"[ClipRegionController] Setting global shader property `_ClipRegion` to: {_clipRegion}");
            Shader.SetGlobalVector("_ClipRegion", _clipRegion);
        }

        public Vector4 GetClipRegion()
        {
            return _clipRegion;
        }
    }
}