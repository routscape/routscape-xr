using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;

public class SelectHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool _clicked = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClick(PointerEvent eventData)
    {
        if (_clicked)
        {
            _clicked = false;
            return;
        }

        HandRef handData = (HandRef)eventData.Data;
        _clicked = true;
        Debug.Log(handData.Handedness);
    }
}
