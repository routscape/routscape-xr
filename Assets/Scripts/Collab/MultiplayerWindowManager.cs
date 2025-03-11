using Meta.XR.MultiplayerBlocks.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Collab
{
    public class MultiplayerWindowManager : MonoBehaviour
    {
        [SerializeField] private GameObject labelInput;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button createButton;
        [SerializeField] private CustomMatchmaking customMatchmaking;

        private readonly string _roomPassword = "r0utsc4p3";

        private void Start()
        {
            joinButton.onClick.AddListener(JoinRoom);
            createButton.onClick.AddListener(CreateRoom);
        }

        private async void JoinRoom()
        {
            var roomId = labelInput.GetComponent<TMP_InputField>().text;
            Debug.Log("RoomID: " + roomId);

            DisableInput();
            SetInputTextField("Joining room...");

            var joinResult = await customMatchmaking.JoinRoom(roomId, _roomPassword);
            if (joinResult.IsSuccess)
            {
                SetInputTextField(roomId);
            }
            else
            {
                Debug.LogError("Failed to join room");
                SetInputTextField("Failed to join room");
                EnableInput();
            }
        }

        private async void CreateRoom()
        {
            DisableInput();
            SetInputTextField("Creating room...");

            var createResult = await customMatchmaking.CreateRoom(new CustomMatchmaking.RoomCreationOptions
            {
                IsPrivate = false,
                LobbyName = "MultiplayerSession",
                MaxPlayersPerRoom = 2,
                RoomPassword = _roomPassword
            });

            if (createResult.IsSuccess)
            {
                var roomId = createResult.RoomToken;
                Debug.Log("RoomID: " + roomId);
                SetInputTextField(roomId);
            }
            else
            {
                Debug.LogError("Failed to create room");
                SetInputTextField("Failed to create room");
                EnableInput();
            }
        }

        private void SetInputTextField(string textValue)
        {
            labelInput.GetComponent<TMP_InputField>().text = textValue;
        }

        private void DisableInput()
        {
            labelInput.GetComponent<TMP_InputField>().interactable = false;
        }

        private void EnableInput()
        {
            labelInput.GetComponent<TMP_InputField>().interactable = true;
        }
    }
}