using Mapbox.Utils;
using UnityEngine;
using Utils;

public class PinCreationCoordinator : MonoBehaviour
{
    [SerializeField] private NetworkEventDispatcher networkEventDispatcher;
    [SerializeField] private MapObjectsManager mapObjectsManager;
    [SerializeField] private UIManager uiManager;
    
    void Start()
    {
        networkEventDispatcher.OnPinDrop += AddPin;
    }

    void AddPin(string pinName, Vector3 position, int objectCategory)
    {
        var pinData = new PinData(pinName, position, (MapObjectCategory)objectCategory);
        mapObjectsManager.AddPin(pinData);
        uiManager.AddPin(pinData);
    }
}
