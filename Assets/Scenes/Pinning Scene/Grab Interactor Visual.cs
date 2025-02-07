using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using TMPro;
using UnityEngine;

public class GrabInteractorVisual : MonoBehaviour
{
    [SerializeField] private HandGrabInteractor rightHandGrabInteractor;
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private TextMeshPro text;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (rightHandGrabInteractor.State == InteractorState.Disabled)
        {
            renderer.material.color = Color.red;
        } else if (rightHandGrabInteractor.State == InteractorState.Normal)
        {
            renderer.material.color = Color.green;
        } else if (rightHandGrabInteractor.State == InteractorState.Hover)
        {
            renderer.material.color = Color.yellow;
        } else if (rightHandGrabInteractor.State == InteractorState.Select)
        {
            renderer.material.color = Color.blue;
        }
        
        text.text = "Grab: " + rightHandGrabInteractor.State;
    }
}
