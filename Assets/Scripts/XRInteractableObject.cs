using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TestEventScript : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _isSelected = false;
    private bool _isHovered = false;
    private MeshRenderer renderer;
    public Material DefaultMaterial;
    public Material HoveredMaterial;
    public Material SelectedMaterial;
    
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
        interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.selectExited.AddListener(OnSelectExited);
    }
    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        _isHovered = true;
        renderer.material = HoveredMaterial;
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        _isHovered = false;
        if(!_isSelected)
            renderer.material = DefaultMaterial;
    }
    
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        _isSelected = true;
        renderer.material = SelectedMaterial;
    }
    
    private void OnSelectExited(SelectExitEventArgs args)
    {
        _isSelected = false;
        renderer.material = DefaultMaterial;
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
