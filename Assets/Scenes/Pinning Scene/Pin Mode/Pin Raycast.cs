using System.Linq;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class PinRaycast : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject mapPin;
    private RaycastHit hitInfo;
    
    void FixedUpdate()
    {
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hitInfo, 100f);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hitInfo.point);
    }

    public void OnDrop(PointerEvent eventData)
    {
        Instantiate(mapPin, hitInfo.point, Quaternion.identity);
        Destroy(transform.parent.parent.gameObject);
    }
}
