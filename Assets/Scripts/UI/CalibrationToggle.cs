using UnityEngine;
using UnityEngine.UI;

public class CalibrationToggle : MonoBehaviour
{
    [SerializeField] private Sprite activeImage;
    [SerializeField] private Sprite inactiveImage;
    [SerializeField] private SpawnPinUI spawnPinUI;
    [SerializeField] private GameObject[] gameObjects;
    private bool isActive;

    private Button itemButton;
    private Image itemImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        itemButton = GetComponent<Button>();
        itemImage = GetComponent<Image>();

        if (itemButton != null) itemButton.onClick.AddListener(OnButtonClick);

        SetItemState(true);
    }

    private void OnButtonClick()
    {
        foreach (var go in gameObjects) go.SetActive(!go.activeSelf);
        if (isActive) spawnPinUI.SpawnUI();
        else spawnPinUI.HideUI();

        SetItemState(!isActive);
    }

    private void SetItemState(bool newState)
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