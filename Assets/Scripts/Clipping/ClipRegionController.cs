using UnityEngine;

namespace Clipping
{
    public class ClipRegionController : MonoBehaviour
    {
        [SerializeField] private Vector4 clipRegion;

        private void OnEnable()
        {
            Shader.SetGlobalVector("_ClipRegion", clipRegion);
        }

        public void SetClipRegion(Vector4 newClipRegion)
        {
            clipRegion = newClipRegion;

            Debug.Log($"[ClipRegionController] Setting global shader property `_ClipRegion` to: {clipRegion}");
            Shader.SetGlobalVector("_ClipRegion", clipRegion);
        }

        public Vector4 GetClipRegion()
        {
            return clipRegion;
        }
    }
}