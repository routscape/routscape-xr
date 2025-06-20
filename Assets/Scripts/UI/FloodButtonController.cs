using System;
using UnityEngine;
using UnityEngine.UI;

public class FloodButtonController: MonoBehaviour
{
    [SerializeField] private GameObject floodCubeManagerParent;
    [SerializeField] private Button floodButtonGraphic;
    [SerializeField] private GameObject floodEditWindow;
    [SerializeField] private FloodSliderController baseSlider;
    [SerializeField] private FloodSliderController coarseSlider;
    
    private NetworkEventDispatcher _networkEventDispatcher;
    private bool _showFlood;
    private static readonly Color ActiveNormal = new Color(0.31f, 0.88f, 0.28f, 180f / 255f);
    private static readonly Color ActiveHighlighted = new Color(0.41f, 0.98f, 0.38f, 200f / 255f); 
    private static readonly Color ActivePressed = new Color(0.21f, 0.78f, 0.18f, 220f / 255f);
    private static readonly Color InactiveNormal = new Color(0, 0, 0, 81f / 255f);
    private static readonly Color Highlighted = new Color(0, 0, 0, 110f / 255f);
    private static readonly Color Pressed = new Color(0, 0, 0, 175f / 255f);

    private static ColorBlock _inactiveColorBlock;
    private static ColorBlock _activeColorBlock;
    private float _lastFrame;
    private int _numSlidersSpawned;

    private void Start()
    {
        _networkEventDispatcher = GameObject.FindWithTag("network event dispatcher").GetComponent<NetworkEventDispatcher>();
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

        _networkEventDispatcher.OnToggleFlood += ToggleFlood;
        baseSlider.OnSpawned += OnSliderSpawned;
        coarseSlider.OnSpawned += OnSliderSpawned;
    }

    private void OnSliderSpawned()
    {
        _numSlidersSpawned++;

        if (_numSlidersSpawned == 2)
        {
            floodEditWindow.SetActive(false);
        }
    }

    public void OnFloodClicked()
    {
        if (HasInputFiredTwice())
        {
            return;
        }
        _networkEventDispatcher.RPC_ToggleFlood();
    }

    void ToggleFlood()
    {
        _showFlood = !_showFlood;

        if (_showFlood)
        {
            floodButtonGraphic.colors = _activeColorBlock;
            floodEditWindow.SetActive(true);
            floodCubeManagerParent.SetActive(true);
        }
        else
        {
            floodButtonGraphic.colors = _inactiveColorBlock; 
            floodEditWindow.SetActive(false);
            floodCubeManagerParent.SetActive(false);
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
