using TMPro;
using UnityEngine;

public class PoiPin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI windowTitle;

    public void SetLabel(string title)
    {
        if (windowTitle != null)
        {
            windowTitle.text = title;
        }
    }
}