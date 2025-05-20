using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform pinListTransform;
    [SerializeField] private Transform routeListTransform;
    [SerializeField] private GameObject listItemPrefab;
    
    private NetworkEventDispatcher _networkEventDispatcher;
    private Dictionary<int, ListItemController> _listItems = new Dictionary<int, ListItemController>();
    void Start()
    {
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
        if (_networkEventDispatcher== null)
        {
            Debug.Log("[UIManager] No network event dispatcher found!");
            throw new Exception("[UIManager] No network event dispatcher found!");
        }

        _networkEventDispatcher.OnHighlightListItem += HighlightListItem;
    }

    public void AddPin(PinData pinData)
    {
        var instantiatedListItem = Instantiate(listItemPrefab, pinListTransform);
        var listItemController = instantiatedListItem.GetComponent<ListItemController>();
        listItemController.SetItemText(pinData.Name);
        listItemController.SetItemColor(pinData.PinColorType);
        listItemController.AddListener(() => {OnClick(pinData.ID);});
        _listItems[pinData.ID] = listItemController;
    }
    public void AddRoute(RouteData routeData)
    {
        var instantiatedListItem = Instantiate(listItemPrefab, routeListTransform);
        var listItemController = instantiatedListItem.GetComponent<ListItemController>();
        listItemController.SetItemText(routeData.Name);
        listItemController.SetItemColor(routeData.RouteColorType);
        listItemController.AddListener(() => {OnClick(routeData.ID);});
        _listItems[routeData.ID] = listItemController;
    }
    
    public void OnClick(int objectID)
    {
        var listItemController = _listItems[objectID];
        if (listItemController.state == "default")
        {
            ResetActiveListItem();
            _networkEventDispatcher.RPC_JumpToMapObject(objectID);
            _networkEventDispatcher.RPC_HighlightListItem(objectID, "selected");
            listItemController.state = "selected";
        } else if (listItemController.state == "selected")
        {
            HighlightListItem(objectID, "edit");
            listItemController.state = "edit";
            //TODO: Modify edit ui of respective object
        } else if (listItemController.state == "edit")
        {
            _networkEventDispatcher.RPC_HighlightListItem(objectID, "default");
            listItemController.state = "default";
        }
    }

    void HighlightListItem(int objectID, string listColorType)
    {
        var listItemController = _listItems[objectID];
        listItemController.SetBackgroundColor(listColorType);
    }
    
    void ResetActiveListItem()
    {
        foreach(var item in _listItems)
        {
            var objectID = item.Key;
            var listItemController = item.Value;
            if (listItemController.state != "default")
            {
                _networkEventDispatcher.RPC_HighlightListItem(objectID, "default");
                listItemController.state = "default";
                break;
            }
        }
    }
}
