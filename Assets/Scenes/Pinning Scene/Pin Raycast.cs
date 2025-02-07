using UnityEngine;

public class PinRaycast : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    void FixedUpdate()
    {
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit hitInfo, 100f);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hitInfo.point);
    }
}
