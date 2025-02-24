using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using TMPro;
using UnityEngine;

public class RayInteractorVisual : MonoBehaviour
{
    [SerializeField] private RayInteractor rightHandRayInteractor;
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private TextMeshPro text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (rightHandRayInteractor.State == InteractorState.Disabled)
        {
            renderer.material.color = Color.red;
        } else if (rightHandRayInteractor.State == InteractorState.Normal)
        {
            renderer.material.color = Color.green;
        } else if (rightHandRayInteractor.State == InteractorState.Hover)
        {
            renderer.material.color = Color.yellow;
        } else if (rightHandRayInteractor.State == InteractorState.Select)
        {
            renderer.material.color = Color.blue;
        }

        text.text = "Ray: " + rightHandRayInteractor.State;
    }
}
