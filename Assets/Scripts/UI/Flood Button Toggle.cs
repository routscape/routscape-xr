using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloodButtonToggle : MonoBehaviour
{
    [SerializeField] private Sprite activeImage;
    [SerializeField] private Sprite inactiveImage;
    [SerializeField] private GameObject floodCube;
    [SerializeField] private GameObject mapMesh;
    private Button itemButton;
    private Image itemImage;
    private bool isActive;

    void Start()
    {
        itemButton = GetComponent<Button>();
        itemImage = GetComponent<Image>();
        
        if (itemButton != null)
        {
            itemButton.onClick.AddListener(OnButtonClick);
        }

        SetItemState(false);
    }
    
    private void OnButtonClick()
    {
        Debug.Log("[Flood Button] Toggle Flood");
        SetItemState(!isActive);
        floodCube.GetComponent<MeshRenderer>().enabled = isActive;
        floodCube.GetComponent<BoxCollider>().enabled = isActive;
        mapMesh.SetActive(!isActive);
    }

    public void ToggleFlood(bool state)
    {
        //Avoid overriding existing user settings
        //If flood is hidden and state is true (try to override), return
        if (!isActive && state)
        {
            return;
        }
        floodCube.GetComponent<MeshRenderer>().enabled = state;
        floodCube.GetComponent<BoxCollider>().enabled = state;
        mapMesh.SetActive(!state);
        GetComponent<Button>().interactable = state;
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