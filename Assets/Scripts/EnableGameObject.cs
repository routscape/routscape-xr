using UnityEngine;

public class EnableGameObject : MonoBehaviour
{
    [SerializeField] private GameObject target;

    public void OnTrigger()
    {
        target.SetActive(true);
    }
}