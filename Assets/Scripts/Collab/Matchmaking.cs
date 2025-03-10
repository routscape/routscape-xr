using System.Threading.Tasks;
using Meta.XR.MultiplayerBlocks.Shared;
using UnityEngine;

namespace Collab
{
    public class Matchmaking : MonoBehaviour
    {
        [SerializeField] private CustomMatchmaking customMatchmaking;
        [SerializeField] private string roomName = "RoutScape";

        private void Start()
        {
            var roomJoinTask = InitializeRoomConnection();
            if (!roomJoinTask.Result) Debug.LogError("[Matchmaking] Failed to initialize room connection");
        }

        private async Task<bool> InitializeRoomConnection()
        {
            // Try to join first
            var joinResult = await JoinRoom(roomName);
            if (!joinResult.IsSuccess)
            {
                // If joining fails, create a room
                var createResult = await CreateRoom();
                if (!createResult.IsSuccess)
                {
                    Debug.LogError("[Matchmaking] Failed to create room");
                    return false;
                }

                Debug.Log("[Matchmaking] Room created");

                // Try to join again
                joinResult = await JoinRoom(roomName);
                if (!joinResult.IsSuccess)
                {
                    Debug.LogError("[Matchmaking] Failed to join room");
                    return false;
                }
            }

            Debug.Log("[Matchmaking] Room joined");
            return true;
        }

        private async Task<CustomMatchmaking.RoomOperationResult> CreateRoom()
        {
            return await customMatchmaking.CreateRoom(new CustomMatchmaking.RoomCreationOptions
            {
                LobbyName = roomName,
                IsPrivate = false,
                MaxPlayersPerRoom = 2
            });
        }

        private async Task<CustomMatchmaking.RoomOperationResult> JoinRoom(string roomName)
        {
            return await customMatchmaking.JoinOpenRoom(roomName);
        }
    }
}