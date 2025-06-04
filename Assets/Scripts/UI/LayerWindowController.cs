using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using Utils;

public class LayerWindowController : MonoBehaviour
{
    [SerializeField] private GameObject listItem;
    [SerializeField] private Transform listItemParent;
    
    private Dictionary<MapObjectCategory, ListItemController> _listItems = new Dictionary<MapObjectCategory, ListItemController>();
    private int _lastFrame;
    void Start()
    {
        if (MapObjectCatalog.I == null)
        {
            return;
        }


        int i = 0;
        foreach (var item in MapObjectCatalog.I.mapObjectTypes)
        {
            var instantiatedListItem = Instantiate(listItem, listItemParent);
            var listItemController = instantiatedListItem.GetComponent<ListItemController>();
            listItemController.SetItemText(item.displayName);
            listItemController.SetIcon(item.icon);
            listItemController.SetItemColor(ColorType.None);
            listItemController.AddListener(() => {OnClick(item.objectCategory);});
            _listItems[item.objectCategory] = listItemController;
            i++;
        }
    }

    void OnClick(MapObjectCategory objectCategory)
    {
        if (HasInputFiredTwice())
        {
            return;
        }
        
        var listItemController = _listItems[objectCategory];
        if (listItemController.state == "default")
        {
            listItemController.SetBackgroundColor("selected");
            listItemController.state = "selected";
            LayerStateManager.I.SetLayerState(objectCategory, false);
        } else if (listItemController.state == "selected")
        {
            listItemController.SetBackgroundColor("default");
            listItemController.state = "default";
            LayerStateManager.I.SetLayerState(objectCategory, true);
        }
    }
    
    private bool HasInputFiredTwice()
    {
        //Hacky solution because why do poke events fire twice?!
        if (_lastFrame == Time.frameCount)
        {
            return true;
        }
        _lastFrame = Time.frameCount;
        return false;
    }
}

