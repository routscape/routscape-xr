using UnityEngine;

public class ChangeTransform : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 position = Vector3.zero;
    [SerializeField] private Quaternion rotation = Quaternion.identity;
    [SerializeField] private Vector3 scale = Vector3.one;

    public void Change()
    {
        var target = targetTransform ?? transform;

        target.position = position;
        target.rotation = rotation;
        target.localScale = scale;
    }
}