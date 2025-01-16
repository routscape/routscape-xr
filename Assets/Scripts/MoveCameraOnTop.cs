using UnityEngine;

public class MoveCameraOnTop : MonoBehaviour
{
    public Transform objectToHide;
    public float cameraHeight = 50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnMouseDown()
    {
        if (objectToHide != null) objectToHide.gameObject.SetActive(false);

        // Calculate new position of the camera
        var newPosition = transform.position;
        newPosition.y += cameraHeight;

        // Set the new position of the camera
        Camera.main.transform.position = newPosition;

        // Set camera angle to look at the object
        Camera.main.transform.LookAt(transform);
    }
}