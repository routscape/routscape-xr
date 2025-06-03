using Mapbox.Unity.Map;
using UnityEngine;

public class MapSnapBehavior : MonoBehaviour
{
    [SerializeField] private AbstractMap mapManager;
    void Start()
    {
        mapManager.gameObject.transform.localPosition = Vector3.zero;
        mapManager.OnUpdated += SnapMapToZero;
    }

    private void SnapMapToZero()
    {
       mapManager.gameObject.transform.localPosition = Vector3.zero;
    }
}
