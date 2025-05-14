using System;
using Oculus.Interaction;
using Unity.Mathematics;
using UnityEngine;

public class PinDropperEdit : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material ghostMaterial;
    public float stateChangeDistanceThreshold = 1f;
    public Action<string> OnStateChanged;
    
    private float _distanceReferenceY;
    private bool _hasDistanceReferenceSet;
    private string _mode = "ghost";
    private RaycastHit _hitInfo;
    private NetworkEventDispatcher _networkEventDispatcher;
    void Start()
    {
        stateChangeDistanceThreshold *= 1000f;
    }
    private void FixedUpdate()
    {
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out _hitInfo, 100f);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, _hitInfo.point);
        if (!_hasDistanceReferenceSet)
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
        meshRenderer.material = redMaterial;
        OnStateChanged?.Invoke("drop");
    }
    private void TransitionToGhost()
    {
        _mode = "ghost";
        meshRenderer.material = ghostMaterial;
        OnStateChanged?.Invoke("ghost");
    }
    public void SetReferenceDistanceY(float y)
    {
        _distanceReferenceY = y * 1000f;
        _hasDistanceReferenceSet = true;
    }
    public void OnDrop(PointerEvent eventData)
    {
        if (_mode == "ghost")
        {
           Debug.Log("[PinDropperEdit] Emit Edit!"); 
        } else if (_mode == "drop")
        {
            Debug.Log("[PinDropperEdit] Emit Reposition!"); 
        }
        Destroy(transform.parent.parent.gameObject);
    }
}
