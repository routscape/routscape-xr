using Fusion;
using Gestures;
using UnityEngine;
using UnityEngine.UI;

public class FloodButtonToggle : NetworkBehaviour
{
    [SerializeField] private Sprite activeImage;
    [SerializeField] private Sprite inactiveImage;
    [SerializeField] private GameObject floodCube;
    [SerializeField] private GameObject mapMesh;
    [SerializeField] private MapZoomHandler mapZoomHandler;
    private bool isActive;
    private Button itemButton;
    private Image itemImage;

    private void Start()
    {
        itemButton = GetComponent<Button>();
        itemImage = GetComponent<Image>();

        if (itemButton != null) itemButton.onClick.AddListener(RpcOnButtonClick);

        SetItemState(false);
    }

    [Rpc]
    private void RpcOnButtonClick()
    {
        Debug.Log("[Flood Button] Toggle Flood");
        SetItemState(!isActive);
        floodCube.GetComponent<MeshRenderer>().enabled = isActive;
        floodCube.GetComponent<BoxCollider>().enabled = isActive;
        mapMesh.SetActive(!isActive);
    }

    [Rpc]
    public void RpcToggleFlood(bool state)
    {
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