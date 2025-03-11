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
        var roomId = labelInput.GetComponent<TMP_InputField>().text;
        Debug.Log("RoomID: " + roomId);
        
        // TODO
        SetRoomId(roomId);
        DisableInput();
    }

    void CreateRoom()
    {
        var roomId = labelInput.GetComponent<TMP_InputField>().text;
        Debug.Log("RoomID: " + roomId);
        
        // TODO
        SetRoomId(roomId);
        DisableInput();
    }

    void SetRoomId(string roomId)
    {
        labelInput.GetComponent<TMP_InputField>().text = roomId;
    }

    void DisableInput()
    {
        labelInput.GetComponent<TMP_InputField>().interactable = false;
    }
}
