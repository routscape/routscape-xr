using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TwoStepRadioButtonGroup : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color activeColor;
    
    [SerializeField] private GameObject editPopup;
    [SerializeField] private TextMeshProUGUI editPopupDisplayText;
	[SerializeField] private string editPopupDisplayTextString;
    [SerializeField] private Button editPopupCloseButton;
    [SerializeField] private Button editPopupConfirmButton;

	private Button selectedButton;

    private void Start()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
        
        if (editPopupCloseButton != null)
        {
            editPopupCloseButton.onClick.AddListener(CloseEditPopup);
        }
        
        if (editPopupConfirmButton != null)
        {
            editPopupConfirmButton.onClick.AddListener(ConfirmEditPopup);
        }
    }

    private void OnButtonClicked(Button clickedButton)
    {
        // Open EditPopup when the currently selected button (item with grayed background) is clicked again
        if (clickedButton == selectedButton)
        {
            if (editPopup != null)
            {
                bool isActive = !editPopup.activeSelf;
                
                // Show EditPopup
                editPopup.SetActive(isActive);

                // Change EditPopup window title
                if (editPopup.activeSelf && editPopupDisplayText != null)
                {
                    editPopupDisplayText.text = editPopupDisplayTextString;
                }

                // Update button color
                if (isActive && clickedButton != null)
                {
                    UpdateButtonColor(clickedButton, activeColor);
                }
                else
                {
                    UpdateButtonColor(clickedButton, selectedColor);
                }
            }
        }
        // Pan the map to the selected item?
        else
        {
            selectedButton = clickedButton;

            foreach (Button button in buttons)
            {
                UpdateButtonColor(button, defaultColor);
            }
            
            UpdateButtonColor(selectedButton, selectedColor);
        }
    }

    private void UpdateButtonColor(Button button, Color color)
    {
        Image buttonImage = button.GetComponent<Image>();
        
        if (buttonImage != null)
        {
            buttonImage.color = color;
        }
    }
    
    private void CloseEditPopup()
    {
        // Hide EditPopup
        if (editPopup != null)
        {
            editPopup.SetActive(false);
        }
        
        // Set button color back to "selectedColor"
        if (selectedButton != null)
        {
            UpdateButtonColor(selectedButton, selectedColor);
        }
    }

    private void ConfirmEditPopup()
    {
        // TODO: Process Changes
        CloseEditPopup();
    }
}