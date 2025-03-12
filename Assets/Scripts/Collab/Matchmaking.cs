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
            if (networkRunner.GetComponent<INetworkSceneManager>() == null)
                networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();

            if (networkRunner.GetComponent<INetworkObjectProvider>() == null)
                networkRunner.gameObject.AddComponent<NetworkObjectProviderDefault>();
        }
    }
}