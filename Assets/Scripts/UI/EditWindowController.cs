using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public enum EditWindowState {
    Default,
    Edit
}

[ExecuteAlways]
public class EditWindowController : MonoBehaviour
{
    [SerializeField] private MapObjectsManager mapObjectsManager;
    [SerializeField] private TMP_Dropdown tmpDropdown;
    [SerializeField] private TMP_Text objectText;
    [SerializeField] private TMP_Text windowTitle;
    [SerializeField] private GameObject editActionButtons;
    [SerializeField] private GameObject defaultActionButtons;
    [SerializeField] private EditWindowState state = EditWindowState.Default;
    private void OnValidate() => ChangeState();
    
    public EditWindowState State
    {
        get => state;
        set
        {
            if (state == value) return;
            state = value;
            ChangeState();
        }
    }
    void Start()
    {
        InitializeDropdown();
        ChangeState();
    }

    public void OnDropdownChanged()
    {
        objectText.text = tmpDropdown.options[tmpDropdown.value].text; 
    }
    
    public void OnDefaultConfirm()
    {
        SelectionService.NewMapObjectData.TypeID = GetTypeID(tmpDropdown.options[tmpDropdown.value].text);
        SelectionService.NewMapObjectData.Name = objectText.text;
    }
    
    public void OnDefaultCancel()
    {
        objectText.text = tmpDropdown.options[tmpDropdown.value].text;
    }
    
    void InitializeDropdown()
    {
        var optionDataList = new List<TMP_Dropdown.OptionData>();
        foreach (var mapObjectType in mapObjectsManager.mapObjectTypes)
        {
            var optionData = new TMP_Dropdown.OptionData()
            {
                text = mapObjectType.displayName,
                image = mapObjectType.icon,
                color = Color.white
            };
            optionDataList.Add(optionData);
        }
        tmpDropdown.AddOptions(optionDataList);
    }

    void ChangeState()
    {
        if (state == EditWindowState.Default)
        {
            editActionButtons.SetActive(false);
            defaultActionButtons.SetActive(true);
            windowTitle.text = "New Pin";
        } else if (state == EditWindowState.Edit)
        {
            editActionButtons.SetActive(true);
            defaultActionButtons.SetActive(false);
            windowTitle.text = "Edit Pin";
        }
    }
    
    private int GetTypeID(string objectName)
    {
        return mapObjectsManager.mapObjectTypes.Find(mapObjectType => 
            mapObjectType.name.ToLower() == objectName.ToLower()).typeID;
    }
}
