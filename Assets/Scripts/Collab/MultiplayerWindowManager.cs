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

        private readonly string _lobbyName = "GlobalLobby";

        private void Start()
        {
            joinButton.onClick.AddListener(JoinRoom);
            createButton.onClick.AddListener(CreateRoom);
        }

        private void JoinRoom()
        {
            SetInputTextField("Joining room...");
            customMatchmaking.JoinOpenRoom(_lobbyName);
        }

        public void OnJoinRoom(CustomMatchmaking.RoomOperationResult result)
        {
            if (result.IsSuccess)
            {
                SetInputTextField(result.RoomToken);
                Debug.Log("[Matchmaking] Room joined successfully. Connected to: " +
                          customMatchmaking.ConnectedRoomToken);
            }
            else
            {
                Debug.LogError("[Matchmaking] Failed to join room.");
                SetInputTextField($"Failed to join room {result.RoomToken}");
            }
        }

        private void CreateRoom()
        {
            SetInputTextField("Creating room...");

            customMatchmaking.CreateRoom(new CustomMatchmaking.RoomCreationOptions
            {
                IsPrivate = false,
                LobbyName = _lobbyName,
                MaxPlayersPerRoom = 2
            });
        }

        public void OnCreateRoom(CustomMatchmaking.RoomOperationResult result)
        {
            if (result.IsSuccess)
            {
                SetInputTextField(result.RoomToken);
                Debug.Log(
                    "[Matchmaking] Room created successfully. Connected to: " + customMatchmaking.ConnectedRoomToken);
            }
            else
            {
                Debug.LogError("[Matchmaking] Failed to create room.");
                SetInputTextField("Failed to create room.");
            }
        }

        private void SetInputTextField(string textValue)
        {
            labelInput.GetComponent<TMP_InputField>().text = textValue;
        }
    }
}