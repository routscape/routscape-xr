using System;
using UnityEngine;

public class PinBehavior : MonoBehaviour
{
    [SerializeField] private ItemPickupHandler itemPickupHandler;
    [SerializeField] private MeshRenderer meshRenderer;
    private void Start()
    { 
        itemPickupHandler = GetComponentInChildren<ItemPickupHandler>(); 
        itemPickupHandler.OnInstantiateObject += OnPinPlacementEditSpawned;
    }
    public void Init(PinData pinData)
    {
        gameObject.transform.position = pinData.WorldPosition;
        gameObject.transform.localScale = new Vector3(pinData.WorldScale, pinData.WorldScale, pinData.WorldScale);
        pinData.OnPinDataChanged += UpdatePinData;
    }

    private void UpdatePinData(PinData pinData)
    {
        gameObject.transform.position = pinData.WorldPosition;
        gameObject.transform.localScale = new Vector3(pinData.WorldScale, pinData.WorldScale, pinData.WorldScale);
    }

    private void OnPinPlacementEditSpawned(GameObject go)
    {
        var pinDropperEdit = go.GetComponentInChildren<PinDropperEdit>();
        pinDropperEdit.SetReferenceDistanceY(gameObject.transform.position.y);
        pinDropperEdit.OnStateChanged += ToggleVisibility;
    }

    private void ToggleVisibility(string pinPlacementEditState)
    {
        if (pinPlacementEditState == "ghost")
        {
            meshRenderer.enabled = true;
        } else if (pinPlacementEditState == "drop")
        {
            meshRenderer.enabled = false;
        }
    }
}
