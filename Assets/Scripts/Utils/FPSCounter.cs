using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro;
    private float _currentFPS;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _currentFPS = 1f / Time.deltaTime;
        UpdateFPS();
    }
    
    private void UpdateFPS()
    {
        textMeshPro.text = "Curr FPS: " + Mathf.RoundToInt(_currentFPS);
    }
}
