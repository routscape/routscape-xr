using Meta.XR.MultiplayerBlocks.Shared;
using UnityEngine;
using Behaviour = Fusion.Behaviour;

namespace Collab
{
    public class Matchmaking : Behaviour
    {
        [SerializeField] private CustomMatchmaking customMatchmaking;

        private void OnApplicationQuit()
        {
            customMatchmaking.LeaveRoom();
        }
    }
}