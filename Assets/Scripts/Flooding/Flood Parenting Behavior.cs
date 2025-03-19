using Flooding;
using UnityEngine;

public class FloodParentingBehavior : MonoBehaviour
{
    [SerializeField] private GameObject floodCube;
    [SerializeField] private GameObject mapParent;
    [SerializeField] private GameObject defaultParent;
    [SerializeField] private FloodButtonToggle floodButtonToggle;
    
    private Vector3 initialScale;
    private Vector3 maxScale;
    private int previousZoomWhole;
    void Start()
    {
        floodCube.GetComponent<MeshFilter>().mesh.RecalculateMeshByBounds(new Vector3(1, 1, 1));

        // Store the initial scale and position of the plane
        initialScale = new Vector3(508.10202f, 16.2605667f, 502.716125f);
        maxScale = new Vector3(994.414917f, 32f, 1018.94208f);
        floodCube.transform.SetParent(mapParent.transform, true);
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


        if (currentZoomWhole == 15)
        {
            floodButtonToggle.ToggleFlood(true);
        }
        else
        {
            floodButtonToggle.ToggleFlood(false);

        }
        floodCube.transform.SetParent(mapParent.transform, true);
        previousZoomWhole = currentZoomWhole;
    }
}
