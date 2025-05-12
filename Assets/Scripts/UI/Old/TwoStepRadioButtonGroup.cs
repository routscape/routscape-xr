using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class TwoStepRadioButtonGroup : NetworkBehaviour
{
    [SerializeField] private List<Button> buttons = new();

    [SerializeField] private UserInterfaceManagerScript userInterfaceManager;
    private readonly Color activeColor = new(10f / 255f, 132f / 255f, 1f, 180f / 255f);
    private readonly Color defaultColor = new(0f, 0f, 0f, 0f);
    private readonly Color drawingColor = new(0.31f, 0.88f, 0.28f, 180f / 255f);
    private readonly Color selectedColor = new(0f, 0f, 0f, 81f / 255f);
    private Button activeButton;
    private bool isClickAllowed = true;

    private Button selectedButton;

    public void AddButton(Button button, bool isDrawing)
    {
        buttons.Add(button);
        button.onClick.AddListener(() => OnButtonClicked(button));

        if (isDrawing)
            UpdateButtonColor(button, drawingColor);
        else
            UpdateButtonColor(button, defaultColor);
    }

    public void RemoveAllButton()
    {
        buttons.Clear();
    }

    public void SetNoActive()
    {
        if (selectedButton != null) UpdateButtonColor(selectedButton, defaultColor);

        if (activeButton != null) UpdateButtonColor(activeButton, defaultColor);

        selectedButton = null;
        activeButton = null;
    }

    private void OnButtonClicked(Button clickedButtonFromEvent)
    {
        var index = buttons.FindIndex(button => button == clickedButtonFromEvent);
        RpcToggleMode(index);
    }

    [Rpc]
    private void RpcToggleMode(int index)
    {
        var clickedButton = buttons.ElementAt(index);
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

            var geoData = selectedButton.GetComponent<UIGeodata>();
            if (geoData != null) userInterfaceManager.JumpTo(geoData.latLong);

            foreach (var button in buttons) UpdateButtonColor(button, defaultColor);

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
        var buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Debug.Log($"Updating {button.name} color: {color}");
            buttonImage.color = color;
        }
    }
}