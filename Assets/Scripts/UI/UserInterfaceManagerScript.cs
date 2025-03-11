using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Pinning;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class UserInterfaceManagerScript : NetworkBehaviour
{
    [SerializeField] private GameObject pinItemPrefab;
    [SerializeField] private Transform pinListTransform;

    [SerializeField] private GameObject routeItemPrefab;
    [SerializeField] private Transform routeListTransform;

    [SerializeField] private ParentButtonToggle pinParentButtonToggle;
    [SerializeField] private ParentButtonToggle routeParentButtonToggle;

    [SerializeField] private TwoStepRadioButtonGroup twoStepRadioButtonGroup;

    [SerializeField] private GameObject routeManager;
    [SerializeField] private GameObject editWindow;

    [SerializeField] private Sprite addSprite;
    [SerializeField] private Sprite finishSprite;
    [SerializeField] private Material[] colors;

    public string currentRouteID;
    public string currentPinID;
    public int mode;

    private readonly List<Tuple<Pin, GameObject>> pinList = new();
    private readonly List<Route> routeList = new();
    private AbstractMap _mapManager;
    public Route currentActiveRoute;
    private Button routeAddButton;
    private Image routeAddButtonImage;
    private Transform routeAddButtonTransform;
    private Transform routeManagerTransform;

    private XRRouteDrawer xrRouteDrawer;

    private void Start()
    {
        Debug.Log("Start");
        xrRouteDrawer = routeManager.GetComponent<XRRouteDrawer>();
        _mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
        if (xrRouteDrawer) Debug.Log("not null");

        InitializeEditWindow();

        /* Initialize route window */
        routeManagerTransform = routeManager.GetComponent<Transform>();
        routeAddButtonTransform = routeListTransform.parent.transform.Find("WindowBar/ActionButtons/AddButton");
        routeAddButton = routeAddButtonTransform.GetComponent<Button>();
        routeAddButtonImage = routeAddButtonTransform.GetComponent<Image>();
        routeAddButton.onClick.RemoveAllListeners();
        routeAddButton.onClick.AddListener(AddRoute);

        PersistentPinSpawnHandler.OnPinDrop += AddPin;

        UpdateWindows(); // delete after
    }

    private void InitializeEditWindow()
    {
        var CancelButton = editWindow.transform.Find("Canvas/ActionButtons/CancelButton");
        var ConfirmButton = editWindow.transform.Find("Canvas/ActionButtons/ConfirmButton");
        var DeleteButton = editWindow.transform.Find("Canvas/ActionButtons/DeleteButton");

        var CancelButtonComponent = CancelButton.GetComponent<Button>();
        var ConfirmButtonComponent = ConfirmButton.GetComponent<Button>();
        var DeleteButtonComponent = DeleteButton.GetComponent<Button>();

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
        mode = 1;
        xrRouteDrawer.enabled = true;
        var route = xrRouteDrawer.CreateNewLine();
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
        mode = 0;
        routeManager.GetComponent<RouteManager>().AddSpawnedRoute(currentActiveRoute);
        currentActiveRoute = null;
        xrRouteDrawer.RemoveCurrentRoute();
        xrRouteDrawer.enabled = false;
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
        foreach (var route in routeList)
        {
            Debug.Log(gameObject.name);
            var newRouteUI = Instantiate(routeItemPrefab, routeListTransform);
            newRouteUI.GetComponent<UIGeodata>().itemID = route.Id;
            newRouteUI.GetComponent<UIGeodata>().latLong = route.GetLocation();
            /* Set button toggle */
            var showHideToggle = newRouteUI.transform.Find("RouteItemTop/ShowHideToggle");

            var buttonToggle = showHideToggle.GetComponent<ButtonToggle>();
            buttonToggle.SetUserInterfaceManager(this);
            routeParentButtonToggle.AddButtonToggle(buttonToggle);

            /* Edit color circle */
            var colorCircle = newRouteUI.transform.Find("RouteItemTop/ColorCircle");

            if (colorCircle != null)
            {
                var img = colorCircle.GetComponent<Image>();

                if (img != null) img.color = route.Color;
            }

            /* Edit route label */
            var routeLabel = newRouteUI.transform.Find("RouteItemTop/ItemLabel");

            if (routeLabel != null)
            {
                var tmpText = routeLabel.GetComponent<TextMeshProUGUI>();

                if (tmpText != null) tmpText.text = route.Name;
            }

            /* Add show/hide toggle to group */
            var routeShowHideToggle = newRouteUI.transform.Find("RouteItemTop/ShowHideToggle");

            if (routeShowHideToggle != null)
            {
                var routeShowHideToggleButton = routeShowHideToggle.GetComponent<ButtonToggle>();
                if (routeShowHideToggleButton != null) pinParentButtonToggle.AddButtonToggle(routeShowHideToggleButton);
            }

            /* Add listener */
            var routeButton = newRouteUI.GetComponentInChildren<Button>();
            if (routeButton != null) twoStepRadioButtonGroup.AddButton(routeButton, route == currentActiveRoute);
        }

        AdjustRouteWindowHeight();

        /* Update pin window */
        foreach (var tuple in pinList)
        {
            var pin = tuple.Item1;
            var newPinUI = Instantiate(pinItemPrefab, pinListTransform);
            newPinUI.GetComponent<UIGeodata>().itemID = pin.MapboxPinId;
            newPinUI.GetComponent<UIGeodata>().latLong = pin.LatLong;

            /* Set button toggle */
            var showHideToggle = newPinUI.transform.Find("PinItemTop/ShowHideToggle");

            var buttonToggle = showHideToggle.GetComponent<ButtonToggle>();
            buttonToggle.SetUserInterfaceManager(this);
            pinParentButtonToggle.AddButtonToggle(buttonToggle);

            /* Edit color circle */
            var colorCircle = newPinUI.transform.Find("PinItemTop/ColorCircle");

            if (colorCircle != null)
            {
                var img = colorCircle.GetComponent<Image>();

                if (img != null) img.color = pin.Color;
            }

            /* Edit pin label */
            var pinLabel = newPinUI.transform.Find("PinItemTop/ItemLabel");

            if (pinLabel != null)
            {
                var tmpText = pinLabel.GetComponent<TextMeshProUGUI>();

                if (tmpText != null) tmpText.text = pin.Name;
            }

            /* Edit pin barangay label */
            var pinBarangayLabel = newPinUI.transform.Find("PinItemBottom/PinBarangay");

            if (pinBarangayLabel != null)
            {
                var tmpText = pinBarangayLabel.GetComponent<TextMeshProUGUI>();

                if (tmpText != null) tmpText.text = "";
            }

            /* Add show/hide toggle to group */
            var pinShowHideToggle = newPinUI.transform.Find("PinItemTop/ShowHideToggle");

            if (pinShowHideToggle != null)
            {
                var pinShowHideToggleButton = pinShowHideToggle.GetComponent<ButtonToggle>();
                if (pinShowHideToggleButton != null) pinParentButtonToggle.AddButtonToggle(pinShowHideToggleButton);
            }

            /* Add listener */
            var pinButton = newPinUI.GetComponentInChildren<Button>();
            if (pinButton != null) twoStepRadioButtonGroup.AddButton(pinButton, false);
        }

        AdjustPinWindowHeight();
    }

    public void OpenEditWindow(Button button)
    {
        var itemUI = button.gameObject;
        Transform label;
        Transform colorCircle;

        if (itemUI.name.StartsWith("PinItem"))
        {
            currentPinID = itemUI.GetComponent<UIGeodata>().itemID;
            label = itemUI.transform.Find("PinItemTop/PinLabel");
            colorCircle = itemUI.transform.Find("PinItemTop/ColorCircle");
        }
        else
        {
            currentRouteID = itemUI.GetComponent<UIGeodata>().itemID;
            label = itemUI.transform.Find("RouteItemTop/RouteLabel");
            colorCircle = itemUI.transform.Find("RouteItemTop/ColorCircle");
        }

        if (label != null)
        {
            var labelText = label.GetComponent<TextMeshProUGUI>();
            if (labelText != null)
            {
                /* Set edit window input hint */
                var editWindowHint = editWindow.transform.Find("Canvas/Input/LabelInput/Text Area/Placeholder");
                var editWindowHintText = editWindowHint.GetComponent<TextMeshProUGUI>();
                editWindowHintText.text = labelText.text;
            }
        }

        if (colorCircle != null)
        {
            var img = colorCircle.GetComponent<Image>();

            if (img != null)
            {
                var editWindowColorDropdown = editWindow.transform.Find("Canvas/Input/ColorDropdown");
                var dropdownComponent = editWindowColorDropdown.GetComponent<TMP_Dropdown>();
                var colorHex = "#" + ColorUtility.ToHtmlStringRGB(img.color);

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
                }

                dropdownComponent.RefreshShownValue();
            }
        }

        editWindow.SetActive(true);
    }

    public void JumpTo(Vector2d latLong)
    {
        _mapManager.UpdateMap(latLong);
    }

    public void CloseEditWindow()
    {
        var editWindowLabel = editWindow.transform.Find("Canvas/Input/LabelInput");
        editWindowLabel.GetComponent<TMP_InputField>().text = "";

        if (editWindow.activeSelf) editWindow.SetActive(false);

        twoStepRadioButtonGroup.SetNoActive();
    }

    private void ConfirmEditWindow()
    {
        var editWindowHint =
            editWindow.transform.Find("Canvas/Input/LabelInput/Text Area/Placeholder"); // For pin identification
        var editWindowColorDropdown = editWindow.transform.Find("Canvas/Input/ColorDropdown");
        var editWindowLabel = editWindow.transform.Find("Canvas/Input/LabelInput/Text Area/Text");

        var labelOld = editWindowHint.GetComponent<TextMeshProUGUI>().text;
        var labelNew = editWindowLabel.GetComponent<TextMeshProUGUI>().text;
        var colorDropdownValue = editWindowColorDropdown.GetComponent<TMP_Dropdown>().value;

        /* Update item */
        var tuple = pinList.FirstOrDefault(tuple => tuple.Item1.MapboxPinId == currentPinID);
        if (tuple != null)
        {
            var pin = tuple.Item1;
            var pinObject = tuple.Item2;
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
            }
        }

        var route = routeList.FirstOrDefault(route => route.Name == labelOld);
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
            }

            xrRouteDrawer.UpdateRoute(labelOld);
        }

        UpdateWindows();
        CloseEditWindow();
    }

    [Rpc]
    private void RpcDeleteItem(string itemID, string type)
    {
        if (type == "pin")
        {
            var pin = pinList.FirstOrDefault(tuple => tuple.Item1.MapboxPinId == itemID);
            _mapManager.VectorData.RemovePointsOfInterestSubLayerWithName(itemID);
            pinList.Remove(pin);
        }
        else if (type == "route")
        {
            var route = routeList.FirstOrDefault(route => route.Id == itemID);
            xrRouteDrawer.DeleteRoute(route.Name);
            routeList.Remove(route);
        }

        UpdateWindows();
    }

    private void DeleteEditWindow()
    {
        var pin = pinList.FirstOrDefault(tuple => tuple.Item1.MapboxPinId == currentPinID);
        if (pin != null)
            RpcDeleteItem(currentPinID, "pin");
        else
            RpcDeleteItem(currentRouteID, "route");

        CloseEditWindow();
    }

    public void ShowRoute(string name, bool show)
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
        foreach (Transform child in parent) Destroy(child.gameObject);
    }

    private void AdjustPinWindowHeight()
    {
        var pinListParent = pinListTransform.parent.GetComponent<RectTransform>();

        var windowBarHeight = 128f;
        var pinItemHeight = 128f;
        var quadHeight = 128f;

        var pinCount = pinList.Count;

        var totalHeight = pinCount * pinItemHeight + windowBarHeight + quadHeight;
        pinListParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }

    private void AdjustRouteWindowHeight()
    {
        var routeListParent = routeListTransform.parent.GetComponent<RectTransform>();

        var windowBarHeight = 128f;
        var routeItemHeight = 128f;
        var quadHeight = 128f;

        var routeCount = routeList.Count;

        var totalHeight = routeCount * routeItemHeight + windowBarHeight + quadHeight;
        routeListParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }

    private IEnumerator ResetButton(float delaySeconds)
    {
        routeAddButton.interactable = false;
        yield return new WaitForSeconds(delaySeconds);
        routeAddButton.interactable = true;
    }
}