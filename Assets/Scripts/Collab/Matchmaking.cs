using Fusion;
using UnityEngine;

namespace Collab
{
    public class Matchmaking : MonoBehaviour
    {
        public void OnFusionServerConnected(NetworkRunner networkRunner)
        {
            networkRunner.StartGame(new StartGameArgs
            {
                SessionName = "MultiplayerSession"
            });
        }
    }
}