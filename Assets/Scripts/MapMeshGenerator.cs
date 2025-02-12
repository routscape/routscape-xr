using System.Collections.Generic;
using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using UnityEngine;

public class MapMeshGenerator : MonoBehaviour
{
    private int _expectedTileCount;
    private Mesh _generatedMesh;
    private GameObject _map;
    private MeshCollider _meshCollider;
    private MeshFilter _meshFilter;
    private List<UnityTile> _tiles;

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

        mapManager.OnTilesStarting += IncrementTileCount;
        mapManager.OnTileFinished += AcknowledgeTile;
    }

    private void IncrementTileCount(List<UnwrappedTileId> tileList)
    {
        _expectedTileCount += tileList.Count;
        Debug.Log("MapMeshGenerator: Expected tile count is now " + _expectedTileCount);
    }

    private void AcknowledgeTile(UnityTile tile)
    {
        if (_tiles == null) _tiles = new List<UnityTile>();

        _tiles.Add(tile);
        Debug.Log("MapMeshGenerator: Tile received, count is now " + _tiles.Count);

        if (_tiles.Count >= _expectedTileCount)
        {
            Debug.Log("MapMeshGenerator: All tiles received, updating mesh");

            _expectedTileCount = 0;
            _tiles.Clear();

            UpdateMesh();
        }
    }

    private void UpdateMesh()
    {
        // Clear existing mesh
        _generatedMesh.Clear();

        // Combine all tile meshes into one
        var combine = new CombineInstance[_tiles.Count];
        var i = 0;

        foreach (var tile in _tiles)
        {
            var tileMesh = tile.MeshFilter.sharedMesh;
            combine[i].mesh = tileMesh;
            combine[i].transform = tile.transform.localToWorldMatrix;
            i++;
        }

        _generatedMesh.CombineMeshes(combine, false, false);

        // Update mesh filter and collider
        _meshFilter.mesh = _generatedMesh;
        _meshCollider.sharedMesh = _generatedMesh;
    }
}