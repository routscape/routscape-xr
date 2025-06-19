using Mapbox.Utils;
using UnityEngine;
using Utils;

public class PinCreationCoordinator : MonoBehaviour
{
    [SerializeField] private NetworkEventDispatcher networkEventDispatcher;
    [SerializeField] private MapObjectsManager mapObjectsManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private EditWindowController editWindowController;
    
    void Start()
    {
        networkEventDispatcher.OnPinDrop += AddPin;
    }

    void AddPin(string pinName, double latitude, double longitude, int objectCategory)
    {
        var pinData = new PinData(pinName, new Vector2d(latitude, longitude), (MapObjectCategory)objectCategory);
        mapObjectsManager.AddPin(pinData);
        uiManager.AddPin(pinData);
        editWindowController.SetDefaultName();
    }
}
