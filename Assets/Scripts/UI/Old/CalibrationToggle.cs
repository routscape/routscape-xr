using UnityEngine;
using UnityEngine.UI;

public class CalibrationToggle : MonoBehaviour
{
    [SerializeField] private Sprite activeImage;
    [SerializeField] private Sprite inactiveImage;
    [SerializeField] private Image itemImage;
    [SerializeField] private GameObject[] gameObjects;
    private bool _isActive;
    private float _lastFrame;
    
    private void Start()
    {
        SetItemState(true);
    }

    public void OnClick()
    {
        if (HasInputFiredTwice())
        {
            return;
        }
        
        foreach (var go in gameObjects) go.SetActive(!go.activeSelf);
        SetItemState(!_isActive);
    }

    private void SetItemState(bool newState)
    {
        _isActive = newState;

        // Set item image based on its state
        if (itemImage != null)
        {
            Debug.Log("New item state...");
            itemImage.sprite = _isActive ? activeImage : inactiveImage;
            itemImage.color = Color.black;
        }
    }
    
    private bool HasInputFiredTwice()
    {
        //Hacky solution because why do poke events fire twice?!
        if (_lastFrame == Time.frameCount)
        {
            return true;
        }
        _lastFrame = Time.frameCount;
        return false;
    }
}