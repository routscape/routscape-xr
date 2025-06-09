using UnityEngine;

public class MapObjectDeleteCoordinator : MonoBehaviour
{
    [SerializeField] private MapObjectsManager mapObjectsManager;
    [SerializeField] private UIManager uiManager;
    private NetworkEventDispatcher _networkEventDispatcher;

    void Start()
    {
        _networkEventDispatcher = GameObject.FindGameObjectWithTag("network event dispatcher")
            .GetComponent<NetworkEventDispatcher>();
        _networkEventDispatcher.OnEraseMapObject += OnEraseMapObject;
    }

    void OnEraseMapObject(int objectID)
    {
        mapObjectsManager.DeleteMapObject(objectID);
        uiManager.DeleteItem(objectID);
    }

}
