using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using UnityEngine;

public class MapMeshGenerator : MonoBehaviour
{
    private Mesh _generatedMesh;
    private GameObject _map;
    private MeshCollider _meshCollider;
    private MeshFilter _meshFilter;

    // OnEnable is called when the object is enabled
    private void OnEnable()
    {
        // Find the map object within the children
        _map = transform.Find("Map").gameObject;

        if (_map == null)
        {
            Debug.LogWarning("MapMeshGenerator: Map object not found in children of " + gameObject.name);
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

        var mapManager = _map.GetComponent<AbstractMap>();

        if (mapManager == null)
        {
            Debug.LogWarning("MapMeshGenerator: AbstractMap component not found in " + _map.name);
            enabled = false;
            return;
        }

        Debug.Log("MapMeshGenerator: Initialized on object " + gameObject.name);

        mapManager.OnTileFinished += UpdateMesh;
    }

    private void UpdateMesh(UnityTile tile)
    {
        var tileMesh = tile.MeshFilter.sharedMesh;

        // Combine with this object's mesh
        var combine = new CombineInstance[1];
        combine[0].mesh = tileMesh;
        _generatedMesh.CombineMeshes(combine, true, false);

        // Fix position of the mesh
        _generatedMesh.RecalculateBounds();
        _generatedMesh.RecalculateNormals();

        // Update mesh filter and collider
        _meshFilter.mesh = _generatedMesh;
        _meshCollider.sharedMesh = _generatedMesh;

        // Sanity check: Log mesh vertices count
        Debug.Log("MapMeshGenerator: Tile " + tile.name + " has " + tileMesh.vertexCount + " vertices");
    }
}