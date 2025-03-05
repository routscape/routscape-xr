using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class UserInterfaceManagerScript : MonoBehaviour
{
	[SerializeField] private GameObject pinItemPrefab;
	[SerializeField] private Transform pinListTransform;
	
	[SerializeField] private ParentButtonToggle parentButtonToggle;
	[SerializeField] private TwoStepRadioButtonGroup twoStepRadioButtonGroup;
	
	[SerializeField] private List<Pin> pinList = new List<Pin>();
	[SerializeField] private GameObject editPinWindow;

    void Start()
    {
	    // temp data
	    AddPin("pompom", -1);
	    pinList.Add(new Pin("Pin 1", -1, ColorType.Red));
	    pinList.Add(new Pin("Pin 2",  -2, ColorType.Blue));
	    pinList.Add(new Pin("Pin 3",  -3, ColorType.Green));
	    
		Debug.Log("Start");
		InitializeEditPinWindow();
		UpdatePinWindow();
    }

	private void InitializeEditPinWindow()
	{
		Transform CancelButton = editPinWindow.transform.Find("Canvas/ActionButtons/CancelButton");
		Transform ConfirmButton = editPinWindow.transform.Find("Canvas/ActionButtons/ConfirmButton");
		Transform DeleteButton = editPinWindow.transform.Find("Canvas/ActionButtons/DeleteButton");

		Button CancelButtonComponent = CancelButton.GetComponent<Button>();
		Button ConfirmButtonComponent = ConfirmButton.GetComponent<Button>();
		Button DeleteButtonComponent = DeleteButton.GetComponent<Button>();

		CancelButtonComponent.onClick.AddListener(CloseEditWindow);
		ConfirmButtonComponent.onClick.AddListener(ConfirmEditWindow);
		DeleteButtonComponent.onClick.AddListener(DeleteEditWindow);
	}

	public void AddPin(string pinName, int mapboxPinId)
	{
		/* default pin color is red */
		pinList.Add(new Pin(pinName, mapboxPinId, ColorType.Red));
	}

	private void UpdatePinWindow()
    {
		twoStepRadioButtonGroup.RemoveAllButton();
		RemoveAllChildren(pinListTransform);
		
	    foreach (Pin pin in pinList)
	    {
		    GameObject newPinUI = Instantiate(pinItemPrefab, pinListTransform);
		    
		    /* Edit color circle */
		    Transform colorCircle = newPinUI.transform.Find("PinItemTop/ColorCircle");

		    if (colorCircle != null)
		    {
			    Image img = colorCircle.GetComponent<Image>();
			    
			    if (img != null)
			    {
				    img.color = pin.Color;
			    }
		    }
		    
		    /* Edit pin label */
		    Transform pinLabel = newPinUI.transform.Find("PinItemTop/PinLabel");

		    if (pinLabel != null)
		    {
			    TextMeshProUGUI tmpText = pinLabel.GetComponent<TextMeshProUGUI>();

			    if (tmpText != null)
			    {
				    tmpText.text = pin.Name;
			    }
		    }
		    
		    /* Edit pin barangay label */
		    Transform pinBarangayLabel = newPinUI.transform.Find("PinItemBottom/PinBarangay");

		    if (pinBarangayLabel != null)
		    {
			    TextMeshProUGUI tmpText = pinBarangayLabel.GetComponent<TextMeshProUGUI>();

			    if (tmpText != null)
			    {
				    tmpText.text = "";
			    }
		    }

			/* Add show/hide toggle to group */
			Transform pinShowHideToggle = newPinUI.transform.Find("PinItemTop/ShowHideToggle");

		    if (pinShowHideToggle != null)
		    {
                ButtonToggle pinShowHideToggleButton = pinShowHideToggle.GetComponent<ButtonToggle>();
                if (pinShowHideToggleButton != null)
                {
                    parentButtonToggle.AddButtonToggle(pinShowHideToggleButton);
                }
		    }

		    /* Add listener */
		    Button pinButton = newPinUI.GetComponentInChildren<Button>();
		    if (pinButton != null)
		    {
			    twoStepRadioButtonGroup.AddButton(pinButton);
		    }
	    }

		AdjustParentHeight();
    }

	public void OpenEditWindow(Button button)
	{
		GameObject pinUI = button.gameObject;

		/* Get pin label */
		Transform pinLabel = pinUI.transform.Find("PinItemTop/PinLabel");
		if (pinLabel != null)
		{
			TextMeshProUGUI pinLabelText = pinLabel.GetComponent<TextMeshProUGUI>();
			if (pinLabelText != null)
			{
				/* Set edit window input hint */
				Transform editWindowHint = editPinWindow.transform.Find("Canvas/Input/LabelInput/Text Area/Placeholder");
				TextMeshProUGUI editWindowHintText = editWindowHint.GetComponent<TextMeshProUGUI>();
				editWindowHintText.text = pinLabelText.text;
			}
		}
		
		/* Get pin color */
		Transform colorCircle = pinUI.transform.Find("PinItemTop/ColorCircle");
		if (colorCircle != null)
		    {
			    Image img = colorCircle.GetComponent<Image>();
			    
			    if (img != null)
			    {
					Transform editWindowColorDropdown = editPinWindow.transform.Find("Canvas/Input/ColorDropdown");
					TMP_Dropdown dropdownComponent = editWindowColorDropdown.GetComponent<TMP_Dropdown>();
					string colorHex = "#" + ColorUtility.ToHtmlStringRGB(img.color);

					switch (colorHex)
					{
						case ColorHexCodes.Green:
							dropdownComponent.value = 0;
							break;
						case ColorHexCodes.Blue:
							dropdownComponent.value = 1;
							break;
						case ColorHexCodes.Red:
							dropdownComponent.value = 2;
							break;
						default:
							break;
					}

					dropdownComponent.RefreshShownValue();
			    }
		    }

		editPinWindow.SetActive(true);
	}

	public void CloseEditWindow()
	{
		if (editPinWindow.activeSelf)
		{
			editPinWindow.SetActive(false);
		}

		twoStepRadioButtonGroup.SetNoActive();
	}

	void ConfirmEditWindow()
	{
		/* Get edit window values */
		Transform editWindowHint = editPinWindow.transform.Find("Canvas/Input/LabelInput/Text Area/Placeholder"); // For pin identification
		Transform editWindowColorDropdown = editPinWindow.transform.Find("Canvas/Input/ColorDropdown");
		Transform editWindowLabel = editPinWindow.transform.Find("Canvas/Input/LabelInput/Text Area/Text");

		string pinLabelOld = editWindowHint.GetComponent<TextMeshProUGUI>().text;
		string pinLabelNew = editWindowLabel.GetComponent<TextMeshProUGUI>().text;
		int colorDropdownValue = editWindowColorDropdown.GetComponent<TMP_Dropdown>().value;

		/* Update pin */
		Pin pin = pinList.FirstOrDefault(pin => pin.Name == pinLabelOld);
		if (pin != null)
		{
			pin.Rename(pinLabelNew);
			
			switch(colorDropdownValue)
			{
				case 0:
					pin.ChangeColor(ColorType.Green);
					break;
				case 1:
					pin.ChangeColor(ColorType.Blue);
					break;
				case 3:
					pin.ChangeColor(ColorType.Red);
					break;
				default:
					break;
			}
		}
		
		UpdatePinWindow();
		CloseEditWindow();
	}

	void DeleteEditWindow()
	{
		Transform editWindowHint = editPinWindow.transform.Find("Canvas/Input/LabelInput/Text Area/Placeholder"); // For pin identification		
		string pinLabel = editWindowHint.GetComponent<TextMeshProUGUI>().text;
		Pin pin = pinList.FirstOrDefault(pin => pin.Name == pinLabel);
		pinList.Remove(pin);

		UpdatePinWindow();
		CloseEditWindow();
	}

	public void RemoveAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    void AdjustParentHeight()
    {
        RectTransform pinListParent = pinListTransform.parent.GetComponent<RectTransform>();
    
        float windowBarHeight = 128f;
        float pinItemHeight = 128f;
        float quadHeight = 128f;
    
        int pinCount = pinList.Count;
    
        float totalHeight = (pinCount * pinItemHeight) + windowBarHeight + quadHeight;
        pinListParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }

}
