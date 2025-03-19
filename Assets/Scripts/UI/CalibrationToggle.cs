using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalibrationToggle : MonoBehaviour
{
    [SerializeField] private Sprite activeImage;
    [SerializeField] private Sprite inactiveImage;
    [SerializeField] private GameObject mesh;
    [SerializeField] private GameObject zoomHolder;
    
    private Button itemButton;
    private Image itemImage;
    private bool isActive;

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
        mesh.SetActive(isActive);
        zoomHolder.SetActive(isActive);
        
        SetItemState(!isActive);
    }
    
    public void SetItemState(bool newState)
    {
        isActive = newState;

		// Set item image based on its state
        if (itemImage != null)
        {
            Debug.Log("New item state...");
            itemImage.sprite = isActive ? activeImage : inactiveImage;
        }
    }
}
