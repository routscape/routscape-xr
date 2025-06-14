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
        var material = instantiatedVisual.GetComponent<MeshRenderer>().material;
        material.shader = ShaderReferenceService.DefaultLitShader;
    }
}
