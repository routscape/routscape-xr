using TMPro;
using UnityEngine;

public class PoiPin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI windowTitle;
    [SerializeField] private GameObject canvasObject;
    private RectTransform canvasRectTransform;
    
    private void Awake()
    {
        if (canvasObject != null)
        {
            canvasRectTransform = canvasObject.GetComponent<RectTransform>();
            if (canvasRectTransform == null)
            {
                Debug.LogError("Assigned GameObject does not have a RectTransform component.");
            }
        }
    }
    
    public void SetLabel(string title)
    {
        if (windowTitle != null)
        {
            windowTitle.text = title;
            windowTitle.ForceMeshUpdate();

            int lineCount = Mathf.Max(windowTitle.textInfo.lineCount, 1);
            float fixedMargin = 2f;
            float contentLineHeight = 4.2f;
            
            float newHeight = contentLineHeight * lineCount + 2 * fixedMargin;

            if (canvasRectTransform != null)
            {
                Vector2 size = canvasRectTransform.sizeDelta;
                size.y = newHeight;
                canvasRectTransform.sizeDelta = size;
            }
        }
    }
}