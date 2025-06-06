using UnityEngine;

namespace Flooding
{
    public class FloodCubeManager : MonoBehaviour
    {
        [SerializeField] private GameObject floodCubePrefab;
        [SerializeField] private int gridSize = 64;
        [SerializeField] private Vector4 boundaries;

        private float _cubeSize;

        private void Start()
        {
            GenerateCubes();
        }

        private void GenerateCubes()
        {
            _cubeSize = (boundaries.w - boundaries.x) / gridSize;

            for (var x = 0; x < gridSize; x++)
            for (var z = 0; z < gridSize; z++)
            {
                var position = new Vector3(
                    boundaries.x + x * _cubeSize + _cubeSize / 2,
                    boundaries.y,
                    boundaries.z + z * _cubeSize + _cubeSize / 2
                );

                var floodCube = Instantiate(floodCubePrefab, position, Quaternion.identity);
                floodCube.transform.localScale = new Vector3(_cubeSize, _cubeSize, _cubeSize);
                floodCube.name = $"FloodCube_{x}_{z}";
                floodCube.transform.SetParent(transform);
            }
        }
    }
}