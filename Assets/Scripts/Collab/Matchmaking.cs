using Fusion;
using Meta.XR.MultiplayerBlocks.Shared;
using UnityEngine;

namespace Collab
{
    public class Matchmaking : MonoBehaviour
    {
        [SerializeField] private CustomMatchmaking customMatchmaking;

        private void OnApplicationQuit()
        {
            customMatchmaking.LeaveRoom();
        }

        public void OnFusionServerConnected(NetworkRunner networkRunner)
        {
            networkRunner.StartGame(new StartGameArgs
            {
                SessionName = "MultiplayerSession"
            });
        }
    }
}