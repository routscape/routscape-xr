using UnityEngine;

public class DisableGameObject : MonoBehaviour
{
    [SerializeField] private GameObject target;

    public void OnTrigger()
    {
        target.SetActive(false);
    }
}