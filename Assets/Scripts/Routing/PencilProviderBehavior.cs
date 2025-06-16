using System;
using Oculus.Interaction;
using UnityEngine;

public class PencilProviderBehavior : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private GameObject pencilPrefab;
    [SerializeField] private RouteDrawer routeDrawer;
    private NetworkEventDispatcher _networkEventDispatcher;
    private Transform _pinchPoint;
    private GameObject _instantiatedPencil;

    void Start()
    {
        _networkEventDispatcher =
            GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        if (_networkEventDispatcher == null)
        {
            Debug.Log("[PinDropper] No network event dispatcher found!");
            throw new Exception("[PinDropper] No network event dispatcher found!");
        }

        _pinchPoint = GameObject.FindWithTag("right pinch area").transform;
        _networkEventDispatcher.OnRouteBegin += HidePencil;
        _networkEventDispatcher.OnRouteEnd += ShowPencil;
    }

    public void OnSelect(PointerEvent eventData)
    {
        _instantiatedPencil = Instantiate(pencilPrefab, _pinchPoint);
        _instantiatedPencil.transform.localPosition = new Vector3(0, -0.00630000001f, -0.00889999978f);
        routeDrawer.SetTipPoint(GameObject.FindGameObjectWithTag("pencil tip point").transform);
    }

    public void OnRelease(PointerEvent eventData)
    {
        _instantiatedPencil.Destroy();
    }

    private void HidePencil(string routeName, int objectCategory, int colorType)
    {
        meshRenderer.enabled = false;
        boxCollider.enabled = false;
    }

    private void ShowPencil()
    { 
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
    }

}
