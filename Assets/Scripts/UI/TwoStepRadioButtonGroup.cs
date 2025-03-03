using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TwoStepRadioButtonGroup : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    private Color defaultColor = new Color(0f, 0f, 0f, 0f);
    private Color selectedColor = new Color(0f, 0f, 0f, 81f / 255f);
    private Color activeColor = new Color(10f / 255f, 132f / 255f, 1f, 180f / 255f);
    
    [SerializeField] private GameObject editPopup;
    [SerializeField] private TextMeshProUGUI editPopupDisplayText;
    [SerializeField] private string editPopupDisplayTextString;
    [SerializeField] private Button editPopupCloseButton;
    [SerializeField] private Button editPopupConfirmButton;

    private Button selectedButton;
    private bool isClickAllowed = true;

    private void Start()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
            UpdateButtonColor(button, defaultColor);
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
        if (!isClickAllowed) return;
        isClickAllowed = false;
        StartCoroutine(EnableClickAfterDelay(0.3f));

        if (clickedButton == selectedButton)
        {
            Debug.Log("Selected button clicked");
            if (editPopup != null)
            {
                bool isActive = !editPopup.activeSelf;
                editPopup.SetActive(isActive);

                if (editPopup.activeSelf && editPopupDisplayText != null)
                {
                    editPopupDisplayText.text = editPopupDisplayTextString;
                }
                UpdateButtonColor(clickedButton, isActive ? activeColor : selectedColor);
            }
        }
        else
        {
            Debug.Log("Selected button not clicked");
            selectedButton = clickedButton;
            
            foreach (Button button in buttons)
            {
                UpdateButtonColor(button, defaultColor);
            }
            UpdateButtonColor(clickedButton, selectedColor);
        }
    }

    private IEnumerator EnableClickAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isClickAllowed = true;
    }

    private void UpdateButtonColor(Button button, Color color)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Debug.Log($"Updating {button.name} color: {color}");
            buttonImage.color = color;
        }
    }
    
    private void CloseEditPopup()
    {
        if (editPopup != null)
        {
            editPopup.SetActive(false);
        }
        if (selectedButton != null)
        {
            UpdateButtonColor(selectedButton, selectedColor);
        }
    }

    private void ConfirmEditPopup()
    {
        CloseEditPopup();
    }
}
