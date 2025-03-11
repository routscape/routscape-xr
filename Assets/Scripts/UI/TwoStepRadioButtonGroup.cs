using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TwoStepRadioButtonGroup : MonoBehaviour
{
    [SerializeField] private List<Button> buttons = new List<Button>();
    private Color defaultColor = new Color(0f, 0f, 0f, 0f);
    private Color selectedColor = new Color(0f, 0f, 0f, 81f / 255f);
    private Color activeColor = new Color(10f / 255f, 132f / 255f, 1f, 180f / 255f);
	private Color drawingColor = new Color(0.31f, 0.88f, 0.28f, 180f / 255f);
    
	[SerializeField] private UserInterfaceManagerScript userInterfaceManager;

    private Button selectedButton;
	private Button activeButton;
    private bool isClickAllowed = true;

	public void AddButton(Button button, bool isDrawing)
	{
		buttons.Add(button);
		button.onClick.AddListener(() => OnButtonClicked(button));

		if (isDrawing)
		{
			UpdateButtonColor(button, drawingColor);
		} else
		{
			UpdateButtonColor(button, defaultColor);
		}
	}

	public void RemoveAllButton()
	{
		buttons.Clear();
	}

	public void SetNoActive()
	{
		if (selectedButton != null)
		{
			UpdateButtonColor(selectedButton, defaultColor);
		}

		if (activeButton != null)
		{
			UpdateButtonColor(activeButton, defaultColor);
		}

		selectedButton = null;
		activeButton = null;
	}

    private void OnButtonClicked(Button clickedButton)
    {
        if (!isClickAllowed) return;
        isClickAllowed = false;
        StartCoroutine(EnableClickAfterDelay(0.3f));

		/* disable button click if drawing route */
		if (userInterfaceManager.currentActiveRoute != null) return;

        if (clickedButton == selectedButton)
        {
            Debug.Log("Selected button clicked");
			selectedButton = null;
			activeButton = clickedButton;
			userInterfaceManager.OpenEditWindow(clickedButton);
            UpdateButtonColor(clickedButton, activeColor);
        }
		else if (clickedButton == activeButton) 
		{
			Debug.Log("Active button clicked");
			activeButton = null;
			UpdateButtonColor(clickedButton, defaultColor);
		}
        else
        {
			userInterfaceManager.CloseEditWindow();
            selectedButton = clickedButton;
            
			var pinId = selectedButton.GetComponent<PinID>();
			if (pinId != null) userInterfaceManager.JumpToPin(selectedButton.GetComponent<PinID>().latLong);

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
}
