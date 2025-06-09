using System;
using Oculus.Interaction;
using Unity.Mathematics;
using UnityEngine;

public class PinDropperEdit : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    
    private Material _dropMaterial;
    private Material _ghostMaterial;
    public float stateChangeDistanceThreshold = 1f;
    public Action<string> OnStateChanged;
    public MeshRenderer meshRenderer;
    private float _distanceReferenceY;
    private bool _hasDistanceReferenceSet;
    private string _mode = "ghost";
    private RaycastHit _hitInfo;
    private NetworkEventDispatcher _networkEventDispatcher;
    private bool _hasMeshInit;

    void Start()
    {
        stateChangeDistanceThreshold *= 1000f;
        _networkEventDispatcher =
            GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
    }
    
    private void FixedUpdate()
    {
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out _hitInfo, 100f);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, _hitInfo.point);
        if (!_hasDistanceReferenceSet || !_hasMeshInit)
        {
            return;
        }

        var selfY = gameObject.transform.position.y * 1000;
        if (selfY < _distanceReferenceY)
        {
            return;
        }
        
        var difference = math.abs(selfY - _distanceReferenceY);
        if (difference >= stateChangeDistanceThreshold)
        {
            TransitionToDrop();
        }
        else
        {
            TransitionToGhost();
        } 
    }
    
    private void TransitionToDrop()
    {
        _mode = "drop";
        meshRenderer.material = _dropMaterial;
        OnStateChanged?.Invoke("drop");
    }
    
    private void TransitionToGhost()
    {
        _mode = "ghost";
        meshRenderer.material = _ghostMaterial;
        OnStateChanged?.Invoke("ghost");
    }
    
    public void SetReferenceDistanceY(float y)
    {
        _distanceReferenceY = y * 1000f;
        _hasDistanceReferenceSet = true;
    }

    public void SetMeshRenderer(MeshRenderer mr, Material dropMaterial, Material ghostMaterial)
    {
        meshRenderer = mr;
        _hasMeshInit = true;
        _dropMaterial = dropMaterial;
        _ghostMaterial = ghostMaterial;
        TransitionToGhost();
    }
    
    public void OnDrop(PointerEvent eventData)
    {
        if (_mode == "ghost")
        {
           Debug.Log("[PinDropperEdit] Emit Edit!"); 
        } else if (_mode == "drop")
        {
            Debug.Log("[PinDropperEdit] Emit Reposition!"); 
            _networkEventDispatcher.RPC_RepositionPin(SelectionService.EditMapObjectData.ObjectID, _hitInfo.point);
            OnStateChanged?.Invoke("dropped");
        }
        Destroy(transform.parent.parent.gameObject);
    }
}
