using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

public class PinJarCoordinator : MonoBehaviour
{
    [SerializeField] private ItemPickupHandler itemPickupHandler;
    
    void Start()
    {
        itemPickupHandler.OnInstantiateObject += OnObjectInstantiated;
    }

    void OnObjectInstantiated(GameObject go)
    {
        var objectCategory = SelectionService.NewMapObjectData.ObjectCategory;
        var visualPrefab =
            MapObjectCatalog.I.GetTypeInfo(objectCategory).visualPrefab;
        Debug.Log("[PinJarCoordinator] visual prefab: " + visualPrefab);
        var instantiatedVisual = Instantiate(visualPrefab, go.GetNamedChild("Behavior").transform);
        var meshRenderers = instantiatedVisual.GetComponentsInChildren<MeshRenderer>();
        ApplyNonClippedShader(meshRenderers);
    }

    void ApplyNonClippedShader(MeshRenderer[] meshRenderers)
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            
            meshRenderers[i].material.shader = ShaderReferenceService.DefaultLitShader;
        }
    }
}
