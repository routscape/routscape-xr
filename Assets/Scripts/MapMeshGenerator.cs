using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using UnityEngine;

public class MapMeshGenerator : MonoBehaviour
{
    [SerializeField] private GameObject map;

    private Mesh _generatedMesh;
    private MeshCollider _meshCollider;
    private MeshFilter _meshFilter;

    // OnEnable is called when the object is enabled
    private void OnEnable()
    {
        if (map == null)
        {
            Debug.LogWarning("MapMeshGenerator: Map object not serialized in " + gameObject.name);
            enabled = false;
            return;
        }

        _meshFilter = gameObject.GetComponent<MeshFilter>();

        if (_meshFilter == null)
        {
            Debug.LogWarning("MapMeshGenerator: Mesh filter not found in " + gameObject.name);
            enabled = false;
            return;
        }

        _meshCollider = gameObject.GetComponent<MeshCollider>();

        if (_meshFilter == null)
        {
            Debug.LogWarning("MapMeshGenerator: Mesh collider not found in " + gameObject.name);
            enabled = false;
            return;
        }

        // Generate new mesh for this object's mesh filter and mesh collider
        _generatedMesh = new Mesh();
        _meshFilter.mesh = _generatedMesh;
        _meshCollider.sharedMesh = _generatedMesh;

        var mapManager = map.GetComponent<AbstractMap>();

        if (mapManager == null)
        {
            Debug.LogWarning("MapMeshGenerator: AbstractMap component not found in " + map.name);
            enabled = false;
            return;
        }

        Debug.Log("MapMeshGenerator: Initialized on object " + gameObject.name);

        mapManager.OnTileFinished += UpdateMesh;
    }

    private void UpdateMesh(UnityTile tile)
    {
        var tileMeshFilter = tile.MeshFilter;
        tileMeshFilter.transform.position = Vector3.zero;
        tileMeshFilter.transform.rotation = Quaternion.identity;
        tileMeshFilter.transform.localScale = Vector3.one;

        var tileMesh = tileMeshFilter.sharedMesh;

        // Combine with this object's mesh
        var combine = new CombineInstance[1];
        combine[0].mesh = tileMesh;
        combine[0].transform = Matrix4x4.identity;
        _generatedMesh.CombineMeshes(combine, true, false);

        // Update mesh filter and collider
        _meshFilter.mesh = _generatedMesh;
        _meshCollider.sharedMesh = _generatedMesh;

        // Sanity check: Log mesh vertices count
        Debug.Log("MapMeshGenerator: Tile " + tile.name + " has " + tileMesh.vertexCount + " vertices");
    }
}