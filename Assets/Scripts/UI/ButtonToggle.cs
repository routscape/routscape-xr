using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonToggle : MonoBehaviour
{
    [SerializeField] private Sprite activeImage;
    [SerializeField] private Sprite inactiveImage;
	[SerializeField] private UserInterfaceManagerScript userInterfaceManager;
    
    private Button itemButton;
    private Image itemImage;
    private bool isActive;
    public delegate void ItemStateChanged();
    public event ItemStateChanged OnItemStateChanged;
    private bool _clicked = false;
	private string itemName;    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemButton = GetComponent<Button>();
        itemImage = GetComponent<Image>();
        
        if (itemButton != null)
        {
            itemButton.onClick.AddListener(OnButtonClick);
        }

		itemName = GetComponent<Transform>().parent.Find("RouteLabel").GetComponent<TextMeshProUGUI>().text;
        SetItemState(true);
    }

	public void SetUserInterfaceManager(UserInterfaceManagerScript userInterfaceManager)
	{
		this.userInterfaceManager = userInterfaceManager;
	}
    
    private void OnButtonClick()
    {
        SetItemState(!isActive);
    }
    
    public void SetItemState(bool newState)
    {
		userInterfaceManager.ShowRoute(itemName, !isActive);
        isActive = newState;

		// Set item image based on its state
        if (itemImage != null)
        {
            Debug.Log("New item state...");
            itemImage.sprite = isActive ? activeImage : inactiveImage;
        }
        
        OnItemStateChanged?.Invoke();
    }
    
    // Used by ParentButtonToggle
    public bool IsActive()
    {
        return isActive;
    }
}
