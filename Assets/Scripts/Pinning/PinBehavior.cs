using System;
using Oculus.Interaction;
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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("[PinBehavior] Collided with " + collision.gameObject.name);
    }
}
