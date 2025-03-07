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
			UpdateButtonColor(selectedButton, selectedColor);
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
			userInterfaceManager.OpenEditWindow(clickedButton);
            UpdateButtonColor(clickedButton, activeColor);
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
