using Unity.Mathematics;
using UnityEngine;

public class EnableGameObject : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject pinUI;
    [SerializeField] private Transform mapLocation;


    private void SpawnUI()
    {
        var newTransform = mapLocation;
        newTransform.position = new Vector3(newTransform.position.x, newTransform.position.y, newTransform.position.z - 0.5f);
        Instantiate(pinUI, newTransform.position, quaternion.identity);
    }
    public void OnTrigger()
    {
        target.SetActive(true);
        SpawnUI();
    }
}