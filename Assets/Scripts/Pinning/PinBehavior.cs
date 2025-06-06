using System;
using Oculus.Interaction;
using Unity.XR.CoreUtils;
using UnityEngine;

public class PinBehavior : MonoBehaviour
{
    [SerializeField] private ItemPickupHandler itemPickupHandler;
    public PinData PinData;
    public MeshRenderer meshRenderer;
    
    private NetworkEventDispatcher _networkEventDispatcher;
    private void Start()
    { 
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        itemPickupHandler = GetComponentInChildren<ItemPickupHandler>(); 
        itemPickupHandler.OnInstantiateObject += OnPinPlacementEditSpawned;
    }
    public void Init(PinData pinData)
    {
        gameObject.transform.position = pinData.WorldPosition;
        gameObject.transform.localScale = new Vector3(pinData.WorldScale, pinData.WorldScale, pinData.WorldScale);
        pinData.OnPinDataChanged += UpdatePinData;
        pinData.OnDelete += DeleteSelf;
        PinData = pinData;
    }

    public void OnHover(PointerEvent eventData)
    {
        float currentScale = gameObject.transform.localScale.x;
        gameObject.transform.localScale = Vector3.one * currentScale * 1.5f;
    }

    public void ExitHover(PointerEvent eventData)
    {
        float currentScale = gameObject.transform.localScale.x;
        gameObject.transform.localScale = Vector3.one * currentScale / 1.5f;
    }

    private void UpdatePinData(PinData pinData)
    {
        gameObject.transform.position = pinData.WorldPosition;
        gameObject.transform.localScale = new Vector3(pinData.WorldScale, pinData.WorldScale, pinData.WorldScale);
    }

    private void OnPinPlacementEditSpawned(GameObject go)
    {
        var pinDropperEdit = go.GetComponentInChildren<PinDropperEdit>();
        var visualPrefab = MapObjectCatalog.I.mapObjectTypes.Find(mapObjectType => mapObjectType.objectCategory == PinData.ObjectCategory).visualPrefab;
        var instantiatedVisual = Instantiate(visualPrefab, go.GetNamedChild("Behavior").transform);
        var visualMeshRenderer = instantiatedVisual.GetComponentInChildren<MeshRenderer>();
        pinDropperEdit.SetReferenceDistanceY(gameObject.transform.position.y);
        pinDropperEdit.SetMeshRenderer(visualMeshRenderer);
        pinDropperEdit.OnStateChanged += ToggleVisibility;
        SelectionService.EditMapObjectData.ObjectID = PinData.ID;
    }

    private void ToggleVisibility(string pinPlacementEditState)
    {
        if (pinPlacementEditState == "ghost" || pinPlacementEditState == "dropped")
        {
            meshRenderer.enabled = true;
        } else if (pinPlacementEditState == "drop")
        {
            meshRenderer.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("[PinBehavior] Deleting oneself with object ID " + PinData.ID);
        _networkEventDispatcher.RPC_EraseMapObject(PinData.ID);
    }

    private void DeleteSelf()
    {
        Destroy(gameObject);
    }
}
