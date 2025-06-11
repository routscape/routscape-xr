using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;
using Utils;

public enum EditWindowState {
    Default,
    Edit
}

[ExecuteAlways]
public class EditWindowController : MonoBehaviour
{
    [SerializeField] private MapObjectsManager mapObjectsManager;
    [SerializeField] private TMP_Dropdown tmpDropdown;
    [SerializeField] private TMP_InputField tmpInputField;
    [SerializeField] private TMP_Text windowTitle;
    [SerializeField] private EditWindowState state = EditWindowState.Default;
    [SerializeField] private MapObjectCatalog mapObjectCatalog;
    
    void Start()
    {
        mapObjectCatalog.OnMapCatalogInitialized += InitializeDropdown;
        SelectionService.NewMapObjectData.ObjectCategory = MapObjectCategory.Pin;
        SelectionService.NewMapObjectData.Name = "Pin";
        tmpInputField.text = "Pin";
    }

    public void OnDropdownChanged()
    {
        SelectionService.NewMapObjectData.ObjectCategory = GetTypeID(tmpDropdown.options[tmpDropdown.value].text);
        SelectionService.NewMapObjectData.Name = tmpInputField.text;
        tmpInputField.text = tmpDropdown.options[tmpDropdown.value].text; 
    }
    
    public void OnTextValueChanged(string value)
    {
        SelectionService.NewMapObjectData.Name = tmpInputField.text;
    }
    
    void InitializeDropdown()
    {
        var optionDataList = new List<TMP_Dropdown.OptionData>();
        foreach (var mapObjectType in MapObjectCatalog.I.mapObjectTypes)
        {
            if (!mapObjectType.isSpawnableByPinJar) continue;
            
            Debug.Log("[EditWindowController] New Dropdown Object");
            var optionData = new TMP_Dropdown.OptionData()
            {
                text = mapObjectType.displayName,
                image = mapObjectType.icon,
                color = Color.white
            };
            optionDataList.Add(optionData);
        }
        tmpDropdown.ClearOptions();
        tmpDropdown.AddOptions(optionDataList);
    }
    
    
    private MapObjectCategory GetTypeID(string objectName)
    {
        return MapObjectCatalog.I.mapObjectTypes.Find(mapObjectType => 
            mapObjectType.name.ToLower() == objectName.ToLower()).objectCategory;
    }
}
