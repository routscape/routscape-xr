using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FloodManager : MonoBehaviour
{
    [SerializeField] private Button lowerButton;
    [SerializeField] private Button raiseButton;
    [SerializeField] private GameObject floodCube;
    [SerializeField] private GameObject mapParent;
    [SerializeField] private GameObject defaultParent;

    private int floodLevel = 0;
    private float floodStep = 0.2f;
    private Vector3 initialScale;
    private Vector3 maxScale;
    private Vector3 initialPosition;
    private int previousZoomWhole;

    void Start()
    {
        lowerButton.onClick.AddListener(LowerFloodLevel);
        raiseButton.onClick.AddListener(RaiseFloodLevel);

        // Store the initial scale and position of the plane
        initialScale = new Vector3(499.03717f, 32f, 507.057129f);
        initialPosition = floodCube.transform.position;
        maxScale = new Vector3(994.414917f, 32f, 1018.94208f);
        floodCube.transform.SetParent(mapParent.transform, true);
        floodCube.transform.position = new Vector3(-0.540000021f, -21f, -0.0500000007f);
    }
    
    public void RaiseFloodLevel()
    {
        floodLevel++;
        AdjustFloodHeight();
        Debug.Log("Flood raised to: " + floodLevel);
    }
    
    public void LowerFloodLevel()
    {
        if (floodLevel > 0) // Prevent going below the initial level
        {
            floodLevel--;
            AdjustFloodHeight();
            Debug.Log("Flood lowered to: " + floodLevel);
        }
    }

    public void ChangeStateToZoom(int currentZoomWhole)
    {
        Debug.Log("ZOOM: " + currentZoomWhole + " " + previousZoomWhole);
        if (currentZoomWhole > previousZoomWhole)
        {
            floodCube.transform.SetParent(defaultParent.transform, true);
            floodCube.transform.localScale = initialScale;
            Debug.Log("FLOOD: Set to default small...");
        }
        else if (currentZoomWhole < previousZoomWhole)
        {
            floodCube.transform.SetParent(defaultParent.transform, true);
            floodCube.transform.localScale = maxScale;
            Debug.Log("FLOOD: Set to default big...");
        }

        floodCube.transform.SetParent(mapParent.transform, true);
        previousZoomWhole = currentZoomWhole;
        Debug.Log("FLOOD: Set to follow...");
    }

    private void AdjustFloodHeight()
    {
        float newHeight = initialScale.y + (floodStep * floodLevel);

        // Scale the cube
        floodCube.transform.localScale = 
            new Vector3(initialScale.x, newHeight, initialScale.z);

        // Move it up so the bottom stays put
        float heightDifference = (newHeight - initialScale.y) * 0.5f;
        floodCube.transform.position = new Vector3(
            initialPosition.x,
            initialPosition.y + heightDifference,
            initialPosition.z
        );
    }
}