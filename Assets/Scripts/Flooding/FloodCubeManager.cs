using UnityEngine;

namespace Flooding
{
    public class FloodCubeManager : MonoBehaviour
    {
        [SerializeField] private GameObject floodCubePrefab;
        [SerializeField] private int gridSize = 64;
        [SerializeField] private Vector4 boundaries;

        private float _cubeSizeX;
        private float _cubeSizeZ;

        private void Start()
        {
            GenerateCubes();
        }

        private void GenerateCubes()
        {
            _cubeSizeX = (boundaries.y - boundaries.x) / gridSize;
            _cubeSizeZ = (boundaries.w - boundaries.z) / gridSize;

            for (var x = 0; x < gridSize; x++)
            for (var z = 0; z < gridSize; z++)
            {
                var position = new Vector3(
                    boundaries.x + x * _cubeSizeX + _cubeSizeX / 2,
                    0,
                    boundaries.z + z * _cubeSizeZ + _cubeSizeZ / 2
                );

                var cube = Instantiate(floodCubePrefab, position, Quaternion.identity);
                cube.transform.localScale = new Vector3(_cubeSizeX, 1, _cubeSizeZ);
                cube.name = $"FloodCube_{x}_{z}";
                cube.transform.SetParent(transform);
            }
        }
    }
}