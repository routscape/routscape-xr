using UnityEngine;

public class EraserProviderBehavior : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private BoxCollider boxCollider;
    private NetworkEventDispatcher _networkEventDispatcher;
    void Start()
    {
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        _networkEventDispatcher.OnEraseBegin += OnEraseBegin;
        _networkEventDispatcher.OnEraseEnd += OnEraseEnd;
    }

    private void OnEraseBegin()
    {
        meshRenderer.enabled = false;
        boxCollider.enabled = false;
    }

    private void OnEraseEnd()
    {
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
    }
}
