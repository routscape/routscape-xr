using Mapbox.Unity.Map;
using UnityEngine;

public class FloodCubeInitializer : MonoBehaviour
{
    [SerializeField] private AbstractMap map;
    [SerializeField] private GameObject defaultParent;

    private Vector3 defaultScale;

    private bool updatedDefault = false;

    private bool updatedNew = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // transform.SetParent(map.transform, false);
        // defaultScale = transform.localScale;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Debug.Log("Zoom check " + (map.Zoom % 1 == 0));
        // if (map.Zoom % 1 == 0 && !updatedDefault)
        // {
        //     updatedDefault = true;
        //     updatedNew = false;
        //     transform.SetParent(defaultParent.transform, false);
        //     transform.localScale = defaultScale;
        // }
        //
        // if (map.Zoom % 1 != 0 && !updatedNew)
        // {
        //     updatedDefault = false;
        //     updatedNew = true;
        //     transform.SetParent(map.transform, false);
        // }
    }
}
