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
    }
}