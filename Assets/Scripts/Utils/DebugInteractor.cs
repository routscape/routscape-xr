using Oculus.Interaction;
using TMPro;
using UnityEngine;

public class DebugInteractor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField, Interface(typeof(IInteractor))] 
    private Object interactor;
    
    private IInteractor _interactor;
    void Awake()
    {
        _interactor = interactor as IInteractor;
    }
    
    void Start()
    {
        if (meshRenderer == null)
        {
           Debug.LogError("[DebugInteractor] Missing Mesh Renderer for Visual!"); 
        }

        if (interactor == null)
        {
            Debug.LogError("[DebugInteractor] Missing Interactor!"); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_interactor.State == InteractorState.Disabled)
        {
            meshRenderer.material.color = Color.red;
        } else if (_interactor.State == InteractorState.Normal)
        {
            meshRenderer.material.color = Color.green;
        } else if (_interactor.State == InteractorState.Hover)
        {
            meshRenderer.material.color = Color.yellow;
        } else if (_interactor.State == InteractorState.Select)
        {
            meshRenderer.material.color = Color.blue;
        }

        if (textMesh != null)
        {
            textMesh.text = _interactor.State.ToString();
        }
    }
}
