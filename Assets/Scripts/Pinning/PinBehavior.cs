using System;
using Oculus.Interaction;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class PinBehavior : MonoBehaviour
{
    [SerializeField] private ItemPickupHandler itemPickupHandler;
    
    public PinData PinData;
    public GameObject TextGameObject;
    public MeshRenderer meshRenderer;
    
    private NetworkEventDispatcher _networkEventDispatcher;
    private Material _defaultMaterial;
    private Material _ghostMaterial;

    private void Start()
    { 
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        itemPickupHandler = GetComponentInChildren<ItemPickupHandler>(); 
        itemPickupHandler.OnInstantiateObject += OnPinPlacementEditSpawned;

        //material for reposition
        _defaultMaterial = new Material(ShaderReferenceService.DefaultLitShader);
        _defaultMaterial.color = meshRenderer.material.color;
        //same material but transparent, for reposition
        _ghostMaterial = new Material(ShaderReferenceService.DefaultLitShader);
        _ghostMaterial.SetFloat("_Surface", 1);
        _ghostMaterial.SetOverrideTag("RenderType", "Transparent");
        _ghostMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        _ghostMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _ghostMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _ghostMaterial.SetInt("_ZWrite", 0);
        _ghostMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        var ghostColor = meshRenderer.material.color;
        _ghostMaterial.color = new Color(ghostColor.r, ghostColor.g, ghostColor.b, 0.75f);
    }
    public void Init(PinData pinData)
    {
        gameObject.transform.position = pinData.WorldPosition;
        gameObject.transform.localScale = new Vector3(pinData.WorldScale, pinData.WorldScale, pinData.WorldScale);
        pinData.OnPinDataChanged += UpdatePinData;
        pinData.OnDelete += DeleteSelf;
        PinData = pinData;
    }

    public void SetTextComponent(GameObject go)
    {
        TextGameObject = go;
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
        pinDropperEdit.SetMeshRenderer(visualMeshRenderer, _defaultMaterial, _ghostMaterial);
        pinDropperEdit.OnStateChanged += ToggleVisibility;
        SelectionService.EditMapObjectData.ObjectID = PinData.ID;
    }

    private void ToggleVisibility(string pinPlacementEditState)
    {
        if (pinPlacementEditState == "ghost" || pinPlacementEditState == "dropped")
        {
            meshRenderer.enabled = true;
            TextGameObject.SetActive(true);
        } else if (pinPlacementEditState == "drop")
        {
            meshRenderer.enabled = false;
            TextGameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("[PinBehavior] OnCollisionEnter " + collision.gameObject.name);
        if (collision.gameObject.name.ToLower() != "eraser(clone)")
        {
            return;
        }
        
        Debug.Log("[PinBehavior] Deleting oneself with object ID " + PinData.ID);
        _networkEventDispatcher.RPC_EraseMapObject(PinData.ID);
    }

    private void DeleteSelf()
    {
        Destroy(gameObject);
    }
}
