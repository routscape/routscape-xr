using UnityEngine;
using UnityEngine.UI;

public class FloodManager : MonoBehaviour
{
    [SerializeField] private Button lowerButton;
    [SerializeField] private Button raiseButton;
    [SerializeField] private GameObject floodPlane;

    private int floodLevel = 0;
    private float floodStep = 0.2f;
    private Vector3 initialScale;
    private Vector3 initialPosition;

    void Start()
    {
        lowerButton.onClick.AddListener(LowerFloodLevel);
        raiseButton.onClick.AddListener(RaiseFloodLevel);

        // Store the initial scale and position of the plane
        initialScale = floodPlane.transform.localScale;
        initialPosition = floodPlane.transform.position;
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

    private void AdjustFloodHeight()
    {
        float newHeight = initialScale.y + (floodStep * floodLevel);

        // Scale the cube
        floodPlane.transform.localScale = 
            new Vector3(initialScale.x, newHeight, initialScale.z);

        // Move it up so the bottom stays put
        float heightDifference = (newHeight - initialScale.y) * 0.5f;
        floodPlane.transform.position = new Vector3(
            initialPosition.x,
            initialPosition.y + heightDifference,
            initialPosition.z
        );
    }

}