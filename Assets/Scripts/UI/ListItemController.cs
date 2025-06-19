using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ListItemController : MonoBehaviour
{
    [SerializeField] private Image circle;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button button;
    public string state = "default";
    
    private readonly Color _defaultColor = new(0f, 0f, 0f, 0f);
    private readonly Color _selectedColor = new(0f, 0f, 0f, 81f / 255f);
    private readonly Color _editColor = new(10f / 255f, 132f / 255f, 1f, 180f / 255f);
    
    public void AddListener(UnityAction unityAction)
    {
       button.onClick.AddListener(unityAction); 
    }

    public void SetBackgroundColor(string listColorType)
    {
        if (listColorType == "default")
        {
            backgroundImage.color = _defaultColor;
        } else if (listColorType == "selected")
        {
            backgroundImage.color = _selectedColor;
        } else if (listColorType == "edit")
        {
            backgroundImage.color = _editColor;
        }
    }
    public void SetItemText(string text)
    {
        label.text = text;
    }

    public void SetIcon(Sprite icon)
    {
        var oldColor = circle.color;
        circle.sprite = icon;
        circle.color = oldColor;
    }
    
    public void SetItemColor(ColorType colorType)
    {
        circle.color = ColorHexCodes.GetColor(colorType);
    }
}
