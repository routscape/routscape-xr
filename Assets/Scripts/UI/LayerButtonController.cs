using UnityEngine;
using UnityEngine.UI;

public class LayerButtonController : MonoBehaviour
{
    [SerializeField] private Button layerButton;
    [SerializeField] private GameObject layerWindow;
    
    private bool _showLayerWindow;
    private static readonly Color ActiveNormal = new Color(0.31f, 0.88f, 0.28f, 180f / 255f);
    private static readonly Color ActiveHighlighted = new Color(0.41f, 0.98f, 0.38f, 200f / 255f); 
    private static readonly Color ActivePressed = new Color(0.21f, 0.78f, 0.18f, 220f / 255f);
    private static readonly Color InactiveNormal = new Color(0, 0, 0, 81f / 255f);
    private static readonly Color Highlighted = new Color(0, 0, 0, 110f / 255f);
    private static readonly Color Pressed = new Color(0, 0, 0, 175f / 255f);

    private static ColorBlock _inactiveColorBlock;
    private static ColorBlock _activeColorBlock;
    private float _lastFrame;
    
    void Start()
    {
        _inactiveColorBlock.normalColor = InactiveNormal;
        _inactiveColorBlock.highlightedColor = Highlighted;
        _inactiveColorBlock.pressedColor = Pressed;
        _inactiveColorBlock.selectedColor = InactiveNormal;
        _inactiveColorBlock.colorMultiplier = 1f;

        _activeColorBlock.normalColor = ActiveNormal;
        _activeColorBlock.highlightedColor = ActiveHighlighted;
        _activeColorBlock.pressedColor = ActivePressed;
        _activeColorBlock.selectedColor = ActiveNormal;
        _activeColorBlock.colorMultiplier = 1f;
    }

    public void OnLayerClicked()
    {
        if (HasInputFiredTwice())
        {
            return;
        }
        _showLayerWindow = !_showLayerWindow;

        if (_showLayerWindow)
        {
            layerButton.colors = _activeColorBlock;
            layerWindow.SetActive(true);
        }
        else
        {
            layerButton.colors = _inactiveColorBlock; 
            layerWindow.SetActive(false);
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
