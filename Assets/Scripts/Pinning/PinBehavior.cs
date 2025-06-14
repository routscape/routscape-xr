using System;
using System.Collections.Generic;
using Oculus.Interaction;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class PinBehavior : MonoBehaviour
{
    [SerializeField] private ItemPickupHandler itemPickupHandler;
    
    public PinData PinData;
    public GameObject TextGameObject;
    public MeshRenderer[] meshRenderers;
    
    private NetworkEventDispatcher _networkEventDispatcher;
    private Material[]_defaultMaterials;
    private Material[] _ghostMaterials;

    private void Start()
    { 
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        itemPickupHandler = GetComponentInChildren<ItemPickupHandler>(); 
        itemPickupHandler.OnInstantiateObject += OnPinPlacementEditSpawned;
    }

    private void InitializeDefaultMaterials()
    {
        _defaultMaterials = new Material[meshRenderers.Length];
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            _defaultMaterials[i] = new Material(ShaderReferenceService.DefaultLitShader);
            _defaultMaterials[i].color = meshRenderers[i].material.color;
        }
    }

    private void InitializeGhostMaterials()
    {
        _ghostMaterials =  new Material[meshRenderers.Length];
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            //same material but transparent, for reposition
            _ghostMaterials[i] = new Material(ShaderReferenceService.DefaultLitShader);
            _ghostMaterials[i].SetFloat("_Surface", 1);
            _ghostMaterials[i].SetOverrideTag("RenderType", "Transparent");
            _ghostMaterials[i].EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            _ghostMaterials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _ghostMaterials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _ghostMaterials[i].SetInt("_ZWrite", 0);
            _ghostMaterials[i].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            var ghostColor = meshRenderers[i].material.color;
            _ghostMaterials[i].color = new Color(ghostColor.r, ghostColor.g, ghostColor.b, 0.75f);
        }
    }
    
    public void Init(PinData pinData)
    {
        gameObject.transform.position = pinData.WorldPosition;
        gameObject.transform.localScale = new Vector3(pinData.WorldScale, pinData.WorldScale, pinData.WorldScale);
        pinData.OnPinDataChanged += UpdatePinData;
        pinData.OnDelete += DeleteSelf;
        PinData = pinData;
        
        InitializeDefaultMaterials();
        InitializeGhostMaterials();
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
        var visualMeshRenderer = instantiatedVisual.GetComponentsInChildren<MeshRenderer>();
        pinDropperEdit.SetReferenceDistanceY(gameObject.transform.position.y);
        pinDropperEdit.SetMeshRenderer(visualMeshRenderer, _defaultMaterials, _ghostMaterials);
        pinDropperEdit.OnStateChanged += ToggleVisibility;
        SelectionService.EditMapObjectData.ObjectID = PinData.ID;
    }

    private void ToggleVisibility(string pinPlacementEditState)
    {
        if (pinPlacementEditState == "ghost" || pinPlacementEditState == "dropped")
        {
            SetVisibility(true);
            TextGameObject.SetActive(true);
        } else if (pinPlacementEditState == "drop")
        {
            SetVisibility(false);
            TextGameObject.SetActive(false);
        }
    }

    private void SetVisibility(bool visibility)
    {
        foreach (var mr in meshRenderers)
        {
            mr.enabled = visibility;
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
