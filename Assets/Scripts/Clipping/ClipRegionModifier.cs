using Flooding;
using UnityEngine;

namespace Clipping
{
    public class ClipRegionModifier : MonoBehaviour
    {
        [SerializeField] private ClipRegionController clipRegionController;
        [SerializeField] private float gridSize = 1f;
        [SerializeField] private FloodCubeManager floodCubeManager;

        private void Start()
        {
            //-0.5, 0.5, 0, 1
            clipRegionController.SetClipRegion(new Vector4(
                transform.position.x - gridSize / 2,
                transform.position.x + gridSize / 2,
                transform.position.z - gridSize / 2,
                transform.position.z + gridSize / 2
            ));
        }

        private void FixedUpdate()
        {
            UpdateClipRegion();
        }

        private void SetGridSize(float newSize)
        {
            if (newSize <= 0)
            {
                Debug.LogWarning("Grid size must be greater than zero.");
                return;
            }

            gridSize = newSize;
            UpdateClipRegion();
        }

        private void UpdateClipRegion()
        {
            if (clipRegionController == null)
            {
                Debug.LogWarning("ClipRegionController is not assigned.");
                return;
            }

            var size = new Vector4();
            size.x = transform.position.x - gridSize / 2;
            size.y = transform.position.x + gridSize / 2;
            size.z = transform.position.z - gridSize / 2;
            size.w = transform.position.z + gridSize / 2;

            var clipRegion = size;

            if (clipRegionController.GetClipRegion() == clipRegion) return;

            Debug.Log($"[ClipRegionModifier] Updating clip region to: {clipRegion}");

            clipRegionController.SetClipRegion(clipRegion);
            floodCubeManager.SetBoundaries(clipRegion);
        }
    }
}