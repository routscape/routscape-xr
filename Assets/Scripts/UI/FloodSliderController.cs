using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloodSliderController : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Slider slider;
    [SerializeField] private FloodGrabBehavior floodGrabBehavior;
    [Networked]
    [OnChangedRender(nameof(NetworkedSliderValueChanged))]
    private float CurrentSliderValue { get; set; }

    private float _previousSliderValue;
    public override void Spawned()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void NetworkedSliderValueChanged()
    {
        float change;
        float newFloodLevel;
        if (_previousSliderValue > CurrentSliderValue)
        {
            change = _previousSliderValue - CurrentSliderValue;
            newFloodLevel = floodGrabBehavior.currentFloodLevel - change; 
            floodGrabBehavior.SetFloodLevel(newFloodLevel); 
        }
        else
        {
            change = CurrentSliderValue - _previousSliderValue;
            newFloodLevel = floodGrabBehavior.currentFloodLevel + change; 
            floodGrabBehavior.SetFloodLevel(newFloodLevel); 
        }
        _previousSliderValue = CurrentSliderValue;

        if (Object.HasInputAuthority)
        {
            return;
        }
        
        slider.value = CurrentSliderValue;
    }
    private void OnSliderValueChanged(float value)
    {
        CurrentSliderValue = value;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Object.RequestStateAuthority();
    }
}
