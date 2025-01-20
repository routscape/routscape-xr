using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GrandparentButtonToggle : MonoBehaviour
{
    [SerializeField] private Sprite activeImage;
    [SerializeField] private Sprite inactiveImage;
    [SerializeField] private List<ParentButtonToggle> parentButtonToggleList;
    
    private Button grandparentButton;
    private Image grandparentImage;
    private bool areAllParentsActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grandparentButton = GetComponent<Button>();
        grandparentImage = GetComponent<Image>();
        
        if (grandparentButton != null)
        {
            grandparentButton.onClick.AddListener(OnGrandparentButtonClick);
        }
        
        foreach (var parentButtonToggle in parentButtonToggleList)
        {
            if (parentButtonToggle != null)
            {
                parentButtonToggle.OnParentStateChanged += UpdateGrandparentButtonState;
            }
        }

        UpdateGrandparentButtonState();
    }
    
    private void OnGrandparentButtonClick()
    {
        bool newState = !areAllParentsActive;
        
        // Update all item state based on grandparent state
        foreach (var parentButtonToggle in parentButtonToggleList)
        {
            if (parentButtonToggle != null)
            {
                parentButtonToggle.SetAllChildrenState(newState);
            }
        }

        UpdateGrandparentButtonState();
    }
    
    private void UpdateGrandparentButtonState()
    {
        // Update grandparent state based on parent state
        areAllParentsActive = true;
        
        foreach (var parentButtonToggle in parentButtonToggleList)
        {
            if (parentButtonToggle != null && !parentButtonToggle.AreAllItemsActive())
            {
                areAllParentsActive = false;
                break;
            }
        }
        
        // Update grandparent image based on its state
        if (grandparentImage != null)
        {
            grandparentImage.sprite = areAllParentsActive ? activeImage : inactiveImage;
        }
    }
}
