using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Linq;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Modifiers;
using Mapbox.Utils;
using Unity.VisualScripting;
using ColorUtility = UnityEngine.ColorUtility;

public class UserInterfaceManagerScript : MonoBehaviour
{
	[SerializeField] private GameObject pinItemPrefab;
	[SerializeField] private Transform pinListTransform;
	
	[SerializeField] private GameObject routeItemPrefab;
	[SerializeField] private Transform routeListTransform;
	
	[SerializeField] private ParentButtonToggle pinParentButtonToggle;
	[SerializeField] private ParentButtonToggle routeParentButtonToggle;
	
	[SerializeField] private TwoStepRadioButtonGroup twoStepRadioButtonGroup;
	
	[SerializeField] private GameObject routeManager;
	
	[SerializeField] private List<Tuple<Pin, GameObject>> pinList = new List<Tuple<Pin, GameObject>>();
	[SerializeField] private List<Route> routeList = new List<Route>();
	[SerializeField] private GameObject editWindow;
	
	[SerializeField] private Sprite addSprite;
	[SerializeField] private Sprite finishSprite;
	[SerializeField] private Material[] colors;

	private XRRouteDrawer xrRouteDrawer;
	public Route currentActiveRoute;

	private Transform routeAddButtonTransform;
	private Button routeAddButton;
	private Image routeAddButtonImage;
	private AbstractMap _mapManager;

	public string currentPinID;
	
	private Transform routeManagerTransform;
	
    void Start()
    {
		Debug.Log("Start");
		xrRouteDrawer = routeManager.GetComponent<XRRouteDrawer>();
		_mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
		if (xrRouteDrawer)
		{
			Debug.Log("not null");
		}
		
		routeManagerTransform = routeManager.GetComponent<Transform>();

		InitializeEditWindow();
		
		/* Initialize route window */
		routeAddButtonTransform = routeListTransform.parent.transform.Find("WindowBar/ActionButtons/AddButton");
		routeAddButton = routeAddButtonTransform.GetComponent<Button>();
		routeAddButtonImage = routeAddButtonTransform.GetComponent<Image>();
		routeAddButton.onClick.RemoveAllListeners();
		routeAddButton.onClick.AddListener(AddRoute);

		PinRaycast.OnPinDrop += AddPin;
		
		UpdateWindows(); // delete after
    }
    
	private void InitializeEditWindow()
	{
		Transform CancelButton = editWindow.transform.Find("Canvas/ActionButtons/CancelButton");
		Transform ConfirmButton = editWindow.transform.Find("Canvas/ActionButtons/ConfirmButton");
		Transform DeleteButton = editWindow.transform.Find("Canvas/ActionButtons/DeleteButton");

		Button CancelButtonComponent = CancelButton.GetComponent<Button>();
		Button ConfirmButtonComponent = ConfirmButton.GetComponent<Button>();
		Button DeleteButtonComponent = DeleteButton.GetComponent<Button>();

		CancelButtonComponent.onClick.AddListener(CloseEditWindow);
		ConfirmButtonComponent.onClick.AddListener(ConfirmEditWindow);
		DeleteButtonComponent.onClick.AddListener(DeleteEditWindow);
	}

	private void AddPin(string pinID, Vector2d latLong, GameObject pinObjet)
	{
		/* default pin color is red */
		pinList.Add(new Tuple<Pin, GameObject>(new Pin("New Pin", pinID, latLong, ColorType.Red), pinObjet));
		UpdateWindows();
	}

	public void AddRoute()
	{
		Route route = xrRouteDrawer.CreateNewLine("Route " + routeList.Count);
		currentActiveRoute = route;
		routeList.Add(route);
		Debug.Log("RouteList Count:" + routeList.Count);
		UpdateWindows();

		routeAddButtonImage.sprite = finishSprite;
		routeAddButton.onClick.RemoveAllListeners();
		Debug.Log("Listeners removed. Adding FinishRoute...");
		routeAddButton.onClick.AddListener(FinishRoute);
		
		StartCoroutine(ResetButton(0.1f));
	}

	private void FinishRoute()
	{
		currentActiveRoute = null;
		xrRouteDrawer.RemoveCurrentRoute();
		UpdateWindows();
		
		routeAddButtonImage.sprite = addSprite;
		routeAddButton.onClick.RemoveAllListeners();
		routeAddButton.onClick.AddListener(AddRoute);
		
		StartCoroutine(ResetButton(0.1f));
	}

	private void UpdateWindows()
    {
		twoStepRadioButtonGroup.RemoveAllButton();
		RemoveAllChildren(pinListTransform);
		RemoveAllChildren(routeListTransform);
		
		/* Update route window */
		foreach (Route route in routeList)
		{
			Debug.Log(this.gameObject.name);
			GameObject newRouteUI = Instantiate(routeItemPrefab, routeListTransform);
			
			/* Set button toggle */
			Transform showHideToggle = newRouteUI.transform.Find("RouteItemTop/ShowHideToggle");
		    
			ButtonToggle buttonToggle = showHideToggle.GetComponent<ButtonToggle>();
			buttonToggle.SetUserInterfaceManager(this);
			routeParentButtonToggle.AddButtonToggle(buttonToggle);
		    
			/* Edit color circle */
			Transform colorCircle = newRouteUI.transform.Find("RouteItemTop/ColorCircle");

			if (colorCircle != null)
			{
				Image img = colorCircle.GetComponent<Image>();
			    
				if (img != null)
				{
					img.color = route.Color;
				}
			}
		    
			/* Edit route label */
			Transform routeLabel = newRouteUI.transform.Find("RouteItemTop/RouteLabel");

			if (routeLabel != null)
			{
				TextMeshProUGUI tmpText = routeLabel.GetComponent<TextMeshProUGUI>();

				if (tmpText != null)
				{
					tmpText.text = route.Name;
				}
			}

			/* Add show/hide toggle to group */
			Transform routeShowHideToggle = newRouteUI.transform.Find("RouteItemTop/ShowHideToggle");

			if (routeShowHideToggle != null)
			{
				ButtonToggle routeShowHideToggleButton = routeShowHideToggle.GetComponent<ButtonToggle>();
				if (routeShowHideToggleButton != null)
				{
					pinParentButtonToggle.AddButtonToggle(routeShowHideToggleButton);
				}
			}

			/* Add listener */
			Button routeButton = newRouteUI.GetComponentInChildren<Button>();
			if (routeButton != null)
			{
				twoStepRadioButtonGroup.AddButton(routeButton, route == currentActiveRoute);
			}
		}

		AdjustRouteWindowHeight();
		
		/* Update pin window */
	    foreach (var tuple in pinList)
	    {
		    var pin = tuple.Item1;
		    GameObject newPinUI = Instantiate(pinItemPrefab, pinListTransform);
		    newPinUI.GetComponent<PinID>().pinID = pin.MapboxPinId;
		    newPinUI.GetComponent<PinID>().latLong = pin.LatLong;
		    
		    /* Set button toggle */
		    Transform showHideToggle = newPinUI.transform.Find("PinItemTop/ShowHideToggle");
		    
		    ButtonToggle buttonToggle = showHideToggle.GetComponent<ButtonToggle>();
		    buttonToggle.SetUserInterfaceManager(this);
		    pinParentButtonToggle.AddButtonToggle(buttonToggle);
		    
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
                    pinParentButtonToggle.AddButtonToggle(pinShowHideToggleButton);
                }
		    }

		    /* Add listener */
		    Button pinButton = newPinUI.GetComponentInChildren<Button>();
		    if (pinButton != null)
		    {
			    twoStepRadioButtonGroup.AddButton(pinButton, false);
		    }
	    }

		AdjustPinWindowHeight();
    }

	public void OpenEditWindow(Button button)
	{
		GameObject itemUI = button.gameObject;
		Transform label;
		Transform colorCircle;

		if (itemUI.name.StartsWith("PinItem"))
		{
			currentPinID = itemUI.GetComponent<PinID>().pinID;
			label = itemUI.transform.Find("PinItemTop/PinLabel");
			colorCircle = itemUI.transform.Find("PinItemTop/ColorCircle");
		}
		else
		{
			label = itemUI.transform.Find("RouteItemTop/RouteLabel");
			colorCircle = itemUI.transform.Find("RouteItemTop/ColorCircle");
		}

		if (label != null)
		{
			TextMeshProUGUI labelText = label.GetComponent<TextMeshProUGUI>();
			if (labelText != null)
			{
				/* Set edit window input hint */
				Transform editWindowHint = editWindow.transform.Find("Canvas/Input/LabelInput/Text Area/Placeholder");
				TextMeshProUGUI editWindowHintText = editWindowHint.GetComponent<TextMeshProUGUI>();
				editWindowHintText.text = labelText.text;
			}
		}
		
		if (colorCircle != null)
		    {
			    Image img = colorCircle.GetComponent<Image>();
			    
			    if (img != null)
			    {
					Transform editWindowColorDropdown = editWindow.transform.Find("Canvas/Input/ColorDropdown");
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

		editWindow.SetActive(true);
	}

	public void JumpToPin(Vector2d latLong)
	{
		_mapManager.UpdateMap(latLong);
	}
	public void CloseEditWindow()
	{
		Transform editWindowLabel = editWindow.transform.Find("Canvas/Input/LabelInput");
		editWindowLabel.GetComponent<TMP_InputField>().text = "";

		if (editWindow.activeSelf)
		{
			editWindow.SetActive(false);
		}

		twoStepRadioButtonGroup.SetNoActive();
	}

	void ConfirmEditWindow()
	{

		var item = _mapManager.VectorData.GetAllFeatureSubLayers().ElementAt(0);
		var pinModifier = ScriptableObject.CreateInstance<PinModifier>();
		var emptyMeshModifiers = new List<MeshModifier>();         // no mesh modifiers
		var newGameObjectMods = new List<GameObjectModifier>() { pinModifier };

		item.CreateCustomStyle(emptyMeshModifiers, newGameObjectMods);
		item.HasChanged = true;
		_mapManager.UpdateMap();
		
		/* Get edit window values */
		Transform editWindowHint = editWindow.transform.Find("Canvas/Input/LabelInput/Text Area/Placeholder"); // For pin identification
		Transform editWindowColorDropdown = editWindow.transform.Find("Canvas/Input/ColorDropdown");
		Transform editWindowLabel = editWindow.transform.Find("Canvas/Input/LabelInput/Text Area/Text");

		string labelOld = editWindowHint.GetComponent<TextMeshProUGUI>().text;
		string labelNew = editWindowLabel.GetComponent<TextMeshProUGUI>().text;
		int colorDropdownValue = editWindowColorDropdown.GetComponent<TMP_Dropdown>().value;

		/* Update item */
		var tuple = pinList.FirstOrDefault(tuple => tuple.Item1.MapboxPinId == currentPinID);
		if (tuple != null)
		{
			Pin pin = tuple.Item1;
			GameObject pinObject = tuple.Item2;
			pin.Rename(labelNew);

			switch (colorDropdownValue)
			{
				case 0:
					pin.ChangeColor(ColorType.Green);
					pinObject.GetComponentInChildren<MeshRenderer>().material = colors[0];
					break;
				case 1:
					pin.ChangeColor(ColorType.Blue);
					pinObject.GetComponentInChildren<MeshRenderer>().material = colors[1];
					break;
				case 2:
					pin.ChangeColor(ColorType.Red);
					pinObject.GetComponentInChildren<MeshRenderer>().material = colors[2];
					break;
				default:
					break;
			}
		}
			
		Route route = routeList.FirstOrDefault(route => route.Name == labelOld);
		if (route != null)
		{
			route.Rename(labelNew);

			switch (colorDropdownValue)
			{
				case 0:
					route.ChangeColor(ColorType.Green);
					break;
				case 1:
					route.ChangeColor(ColorType.Blue);
					break;
				case 2:
					route.ChangeColor(ColorType.Red);
					break;
				default:
					break;
			}

			xrRouteDrawer.UpdateRoute(labelOld);
		}

		UpdateWindows();
		CloseEditWindow();
	}

	void DeleteEditWindow()
	{
		Transform editWindowHint = editWindow.transform.Find("Canvas/Input/LabelInput/Text Area/Placeholder"); // For pin identification		
		string objectLabel = editWindowHint.GetComponent<TextMeshProUGUI>().text;
		var pin = pinList.FirstOrDefault(tuple => tuple.Item1.MapboxPinId == currentPinID);
		if (pin != null)
		{
			PinRaycast.PinsDropped.Remove(currentPinID);
			_mapManager.VectorData.RemovePointsOfInterestSubLayerWithName(currentPinID);
			pinList.Remove(pin);
		}
		else
		{
			Route route = routeList.FirstOrDefault(route => route.Name == objectLabel);
			routeList.Remove(route);
			xrRouteDrawer.DeleteRoute(route.Name);
		}

		UpdateWindows();
		CloseEditWindow();
	}

	public void ShowRoute(String name, bool show)
	{
		Debug.Log("Name to find: " + name);
		
		foreach (Transform child in routeManagerTransform)
		{
			Debug.Log(child.name);
			if (child.name == name)
			{
				child.gameObject.SetActive(show);
				return;
			}
		}

	}

	public void RemoveAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    void AdjustPinWindowHeight()
    {
        RectTransform pinListParent = pinListTransform.parent.GetComponent<RectTransform>();
    
        float windowBarHeight = 128f;
        float pinItemHeight = 128f;
        float quadHeight = 128f;
    
        int pinCount = pinList.Count;
    
        float totalHeight = (pinCount * pinItemHeight) + windowBarHeight + quadHeight;
        pinListParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }

    void AdjustRouteWindowHeight()
    {
	    RectTransform routeListParent = routeListTransform.parent.GetComponent<RectTransform>();
    
	    float windowBarHeight = 128f;
	    float routeItemHeight = 128f;
	    float quadHeight = 128f;
    
	    int routeCount = routeList.Count;
    
	    float totalHeight = (routeCount * routeItemHeight) + windowBarHeight + quadHeight;
	    routeListParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }
    
    private IEnumerator ResetButton(float delaySeconds)
    {
	    routeAddButton.interactable = false;
	    yield return new WaitForSeconds(delaySeconds);
	    routeAddButton.interactable = true;
    }
}
