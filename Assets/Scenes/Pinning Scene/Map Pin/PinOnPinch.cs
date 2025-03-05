using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;

public class PinOnPinch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject target;

    public void OnClick(PointerEvent eventData)
    {
        Destroy(target);
    }
}
