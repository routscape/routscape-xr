using System;
using Flooding;
using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloodSliderController : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Slider slider;
    [SerializeField] private FloodCubeManager floodCubeManager;
    [SerializeField] private GameObject parent;
    public Action OnSpawned;
    [Networked]
    [OnChangedRender(nameof(NetworkedSliderValueChanged))]
    private float CurrentSliderValue { get; set; }

    private float _previousSliderValue;
    public override void Spawned()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSpawned?.Invoke();
    }

    private void NetworkedSliderValueChanged()
    {
        float change;
        float newFloodLevel;
        if (_previousSliderValue > CurrentSliderValue)
        {
            change = _previousSliderValue - CurrentSliderValue;
            newFloodLevel = floodCubeManager.floodHeight - change; 
            floodCubeManager.SetFloodHeight(newFloodLevel);
        }
        else
        {
            change = CurrentSliderValue - _previousSliderValue;
            newFloodLevel = floodCubeManager.floodHeight + change; 
            floodCubeManager.SetFloodHeight(newFloodLevel);
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
