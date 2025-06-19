using System;
using Flooding;
using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloodSliderController : NetworkBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Slider slider;
    [SerializeField] private FloodCubeManager floodCubeManager;
    [SerializeField] private GameObject parent;
    public Action OnSpawned;
    
    [Networked]
    [OnChangedRender(nameof(NetworkedSliderValueChanged))]
    private float CurrentSliderValue { get; set; }

    private float _previousSliderValue;
    private float _lastFrame;
    public override void Spawned()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSpawned?.Invoke();
    }

    private void NetworkedSliderValueChanged()
    {
        Debug.Log("[FloodSliderController] new slider value!");
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

        if (Object.HasStateAuthority)
        {
            return;
        }
        
        slider.value = CurrentSliderValue;
    }
    private void OnSliderValueChanged(float value)
    {
        if (HasInputFiredTwice())
        {
            return;
        }
        CurrentSliderValue = value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("[FloodSliderController] State Authority Requested");
        Object.RequestStateAuthority();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("[FloodSliderController] State Authority Released");
        Object.ReleaseStateAuthority();
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
