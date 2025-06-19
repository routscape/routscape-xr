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
    private string defaultName;
    
    void Start()
    {
        InitializeDropdown();
        SelectionService.NewMapObjectData.ObjectCategory = MapObjectCategory.Pin;
        SelectionService.NewMapObjectData.Name = "Pin";
        tmpInputField.text = "Pin";
        defaultName = "Pin";
    }

    public void SetDefaultName()
    {
        tmpInputField.text = defaultName;
        SelectionService.NewMapObjectData.Name = defaultName;
    }

    public void OnDropdownChanged()
    {
        tmpInputField.text = tmpDropdown.options[tmpDropdown.value].text;
        defaultName = tmpDropdown.options[tmpDropdown.value].text;
        SelectionService.NewMapObjectData.ObjectCategory = GetTypeID(tmpDropdown.options[tmpDropdown.value].text);
        Debug.Log("[EditWindowController] Object Type: " + SelectionService.NewMapObjectData.ObjectCategory);
        SelectionService.NewMapObjectData.Name = tmpInputField.text;
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
            
            Debug.Log("[EditWindowController] New Dropdown Object with visual " + mapObjectType.visualPrefab);
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
            mapObjectType.displayName.ToLower() == objectName.ToLower()).objectCategory;
    }
}
