using Fusion;
using Meta.XR.MultiplayerBlocks.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Collab
{
    public class Matchmaking : MonoBehaviour
    {
        [SerializeField] private CustomMatchmaking customMatchmaking;

        private void OnApplicationQuit()
        {
            customMatchmaking.LeaveRoom();
        }

        public void LoadActiveScene(NetworkRunner networkRunner)
        {
            var scene = SceneManager.GetActiveScene();
            networkRunner.LoadScene(scene.name);
        }
    }
}