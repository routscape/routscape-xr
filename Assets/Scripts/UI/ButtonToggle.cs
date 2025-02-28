using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    [SerializeField] private Sprite activeImage;
    [SerializeField] private Sprite inactiveImage;
    
    private Button itemButton;
    private Image itemImage;
    private bool isActive;
    public delegate void ItemStateChanged();
    public event ItemStateChanged OnItemStateChanged;
    private bool _clicked = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemButton = GetComponent<Button>();
        itemImage = GetComponent<Image>();
        
        if (itemButton != null)
        {
            itemButton.onClick.AddListener(OnButtonClick);
        }

        SetItemState(true);
    }
    
    private void OnButtonClick()
    {
        SetItemState(!isActive);
    }
    
    public void SetItemState(bool newState)
    {
        if (_clicked)
        {
            _clicked = false;
            return;
        }

        _clicked = true;
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
