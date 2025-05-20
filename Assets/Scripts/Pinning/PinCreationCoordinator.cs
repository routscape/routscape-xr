using Mapbox.Utils;
using UnityEngine;

public class PinCreationCoordinator : MonoBehaviour
{
    [SerializeField] private NetworkEventDispatcher networkEventDispatcher;
    [SerializeField] private MapObjectsManager mapObjectsManager;
    [SerializeField] private UIManager uiManager;
    
    void Start()
    {
        networkEventDispatcher.OnPinDrop += AddPin;
    }

    void AddPin(string pinName, Vector3 position, int colorType)
    {
        var pinData = new PinData(pinName, position, (ColorType)colorType);
        mapObjectsManager.AddPin(pinData);
        uiManager.AddPin(pinData);
    }
}
