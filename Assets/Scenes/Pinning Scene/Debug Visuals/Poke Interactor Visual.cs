using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using TMPro;
using UnityEngine;

public class PokeInteractorVisual : MonoBehaviour
{
    [SerializeField] private PokeInteractor rightHandPokeInteractor;
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private TextMeshPro text;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (rightHandPokeInteractor.State == InteractorState.Disabled)
        {
            renderer.material.color = Color.red;
        } else if (rightHandPokeInteractor.State == InteractorState.Normal)
        {
            renderer.material.color = Color.green;
        }
        
        text.text = "Poke: " + rightHandPokeInteractor.State;
    }
}
