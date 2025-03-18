using Flooding;
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
    public float floodStep = 0.00125f;
    
    private float floodLevel = 1;
    private Vector3 initialScale;
    private Vector3 maxScale;
    private int previousZoomWhole;

    void Start()
    {
        floodCube.GetComponent<MeshFilter>().mesh.RecalculateMeshByBounds(new Vector3(1, 1, 1));
        lowerButton.onClick.AddListener(LowerFloodLevel);
        raiseButton.onClick.AddListener(RaiseFloodLevel);

        // Store the initial scale and position of the plane
        initialScale = new Vector3(499.03717f, 32f, 507.057129f);
        maxScale = new Vector3(994.414917f, 32f, 1018.94208f);
        floodCube.transform.SetParent(mapParent.transform, true);
    }
    
    public void RaiseFloodLevel()
    {
        floodLevel += floodStep;
        AdjustFloodHeight();
    }
    
    public void LowerFloodLevel()
    {
        if (floodLevel > 0) // Prevent going below the initial level
        {
            floodLevel -= floodStep;
            AdjustFloodHeight();
        }
    }

    public void ChangeStateToZoom(int currentZoomWhole)
    {
        if (currentZoomWhole > previousZoomWhole)
        {
            floodCube.transform.SetParent(defaultParent.transform, true);
            floodCube.transform.localScale = initialScale;
        }
        else if (currentZoomWhole < previousZoomWhole)
        {
            floodCube.transform.SetParent(defaultParent.transform, true);
            floodCube.transform.localScale = maxScale;
        }

        floodCube.transform.SetParent(mapParent.transform, true);
        previousZoomWhole = currentZoomWhole;
    }

    private void AdjustFloodHeight()
    {
        floodCube.GetComponent<MeshFilter>().mesh.RecalculateMeshByBounds(new Vector3(1, 1, floodLevel));
    }
}