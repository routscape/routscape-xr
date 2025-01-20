using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ParentButtonToggle : MonoBehaviour
{
    [SerializeField] private Sprite activeImage;
    [SerializeField] private Sprite inactiveImage;
    [SerializeField] private List<ButtonToggle> childButtonToggleList;
    
    private Button parentButton;
    private Image parentImage;
    private bool areAllChildrenActive;

    public delegate void ParentStateChanged();
    public event ParentStateChanged OnParentStateChanged;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parentButton = GetComponent<Button>();
        parentImage = GetComponent<Image>();
        
        if (parentButton != null)
        {
            parentButton.onClick.AddListener(OnParentButtonClick);
        }
        
        foreach (var childButtonToggle in childButtonToggleList)
        {
            if (childButtonToggle != null)
            {
                childButtonToggle.OnItemStateChanged += UpdateParentButtonState;
            }
        }

        UpdateParentButtonState();
    }
    
    private void OnParentButtonClick()
    {
        bool newState = !areAllChildrenActive;

		SetAllChildrenState(newState);
        UpdateParentButtonState();
    }
    
    private void UpdateParentButtonState()
    {
		//  Update parent state based on children state
        areAllChildrenActive = true;
        
        foreach (var childButtonToggle in childButtonToggleList)
        {
            if (childButtonToggle != null && !childButtonToggle.IsActive())
            {
                areAllChildrenActive = false;
                break;
            }
        }
        
		// Update parent image based on its state
        if (parentImage != null)
        {
            parentImage.sprite = areAllChildrenActive ? activeImage : inactiveImage;
        }
        
        OnParentStateChanged?.Invoke();
    }

	public void SetAllChildrenState(bool newState)
    {
        foreach (var childButtonToggle in childButtonToggleList)
        {
            if (childButtonToggle != null)
            {
                childButtonToggle.SetItemState(newState);
            }
        }

        UpdateParentButtonState();
    }
    
    // Used by GrandparentButtonToggle
    public bool AreAllItemsActive()
    {
        return areAllChildrenActive;
    }

    
}
