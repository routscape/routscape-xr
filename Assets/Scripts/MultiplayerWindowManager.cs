using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiplayerWindowManager : MonoBehaviour
{
    [SerializeField] private GameObject labelInput;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button createButton;

    void Start()
    {
        joinButton.onClick.AddListener(JoinRoom);
        createButton.onClick.AddListener(CreateRoom);
    }

    void JoinRoom()
    {
        var roomId = labelInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_InputField>().text;
        Debug.Log(roomId);
        
        // TODO
    }

    void CreateRoom()
    {
        var roomId = labelInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_InputField>().text;
        Debug.Log(roomId);
        
        // TODO
    }
}
