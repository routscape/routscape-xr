using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using UnityEngine;

public class MapMeshGenerator : MonoBehaviour
{
    private Mesh _generatedMesh;
    private GameObject _map;
    private MeshCollider _meshCollider;
    private MeshFilter _meshFilter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Find the map object within the children
        _map = transform.Find("Map").gameObject;

        if (_map == null)
        {
            enabled = false;
            return;
        }

        _meshFilter = gameObject.GetComponent<MeshFilter>();
        _meshCollider = gameObject.GetComponent<MeshCollider>();

        // Generate new mesh for this object's mesh filter and mesh collider
        _generatedMesh = new Mesh();
        _meshFilter.mesh = _generatedMesh;
        _meshCollider.sharedMesh = _generatedMesh;

        var mapManager = _map.GetComponent<AbstractMap>();
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
    }
}