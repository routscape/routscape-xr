using UnityEngine;
using UnityEngine.UI;

public class FloodSliderController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private FloodGrabBehavior floodGrabBehavior;

    private float _previousSliderValue;
    void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        float change;
        float newFloodLevel;
        if (_previousSliderValue > value)
        {
            change = _previousSliderValue - value;
            newFloodLevel = floodGrabBehavior.currentFloodLevel - change; 
            floodGrabBehavior.SetFloodLevel(newFloodLevel); 
        }
        else
        {
            change = value - _previousSliderValue;
            newFloodLevel = floodGrabBehavior.currentFloodLevel + change; 
            floodGrabBehavior.SetFloodLevel(newFloodLevel); 
        }
        _previousSliderValue = value;
    }
}
