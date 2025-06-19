using Oculus.Interaction;
using UnityEngine;

public class EraserBehavior : MonoBehaviour
{
    private NetworkEventDispatcher _networkEventDispatcher;
    void Start()
    {
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        _networkEventDispatcher.RPC_EraseBegin();
    }

    public void OnRelease(PointerEvent eventData)
    {
        _networkEventDispatcher.RPC_EraseEnd();
        Destroy(gameObject);
    }
}
