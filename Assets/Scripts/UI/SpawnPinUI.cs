using UnityEngine;

public class SpawnPinUI : MonoBehaviour
{
    [SerializeField] private GameObject pinUI;
    [SerializeField] private Transform mapLocation;

    public void SpawnUI()
    {
        var newPosition = mapLocation.position;
        newPosition = new Vector3(newPosition.x, newPosition.y + 0.3f, newPosition.z);
        pinUI.SetActive(true);
        pinUI.transform.position = newPosition;
    }

    public void HideUI()
    {
        pinUI.SetActive(false);
    }
}