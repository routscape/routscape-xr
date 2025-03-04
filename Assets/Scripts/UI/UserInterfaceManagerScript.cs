using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UserInterfaceManagerScript : MonoBehaviour
{
	[SerializeField] private GameObject pinItemPrefab;
	[SerializeField] private Transform pinListTransform;
	
	[SerializeField] private ParentButtonToggle parentButtonToggle;
	[SerializeField] private TwoStepRadioButtonGroup twoStepRadioButtonGroup;
	
	[SerializeField] List<Pin> pinList = new List<Pin>();
	private GameObject editPinWindow;

    void Start()
    {
	    // temp data
	    pinList.Add(new Pin("Pin 1", "Barangay A", new Vector3(0, 0, 0), ColorType.Red));
	    pinList.Add(new Pin("Pin 2",  "Barangay B", new Vector3(0, 0, 0), ColorType.Blue));
	    pinList.Add(new Pin("Pin 3",  "Barangay C", new Vector3(0, 0, 0), ColorType.Green));
	    
		Debug.Log("Start");
		UpdatePinWindow();
    }

    private void UpdatePinWindow()
    {
	    Debug.Log("UpdatePinWindow");
	    foreach (Pin pin in pinList)
	    {
		    Debug.Log("Pin instadsjalkds");
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
				    tmpText.text = pin.Barangay;
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

		    AdjustParentHeight();
	    }
    }
    
    void AdjustParentHeight()
    {
	    RectTransform pinListParent = pinListTransform.parent.GetComponent<RectTransform>();
	    
	    float windowBarHeight = 128f;
	    float pinItemHeight = 128f;
	    float quadHeight = 128f;
	    
	    int pinCount = pinListTransform.childCount;

	    float totalHeight = (pinCount * pinItemHeight) + ((pinCount - 1)) + windowBarHeight + quadHeight;
	    
	    pinListParent.sizeDelta = new Vector2(pinListParent.sizeDelta.x, totalHeight);
    }

}
