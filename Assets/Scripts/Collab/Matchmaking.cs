using System.Linq;
using Fusion;
using Meta.XR.MultiplayerBlocks.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Collab
{
    public class Matchmaking : MonoBehaviour
    {
        [SerializeField] private CustomMatchmaking customMatchmaking;
        [SerializeField] private ChangeTransform multiplayerWindowChangeTransform;

        private void OnApplicationQuit()
        {
            customMatchmaking.LeaveRoom();
        }

        public void LoadActiveScene(NetworkRunner networkRunner)
        {
            var scene = SceneManager.GetActiveScene();
            networkRunner.LoadScene(scene.name);
        }

        public void OnPlayerJoin(NetworkRunner networkRunner, PlayerRef player)
        {
            Debug.Log($"Player {player.PlayerId} joined the session");
            if (networkRunner.ActivePlayers.Count() == 2)
            {
                Debug.Log("Two users are in the room. Starting RoutScape...");
                multiplayerWindowChangeTransform.Change();
            }
        }
    }
}