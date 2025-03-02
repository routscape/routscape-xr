using Unity.Mathematics;
using UnityEngine;

public class EnableGameObject : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject pinUI;
    [SerializeField] private Transform mapLocation;


    private void SpawnUI()
    {
        var newPosition = mapLocation.position;
        newPosition = new Vector3(newPosition.x, newPosition.y + 0.3f, newPosition.z);
        pinUI.SetActive(true);
        pinUI.transform.position = newPosition;
    }
    public void OnTrigger()
    {
        target.SetActive(true);
        SpawnUI();
    }
}