using Oculus.Interaction;
using UnityEngine;

public class PinRaycast : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject mapPin;
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
