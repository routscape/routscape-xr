using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Collab
{
    public class PhotonHandler : MonoBehaviourPunCallbacks
    {
        private const string GameVersion = "1";
        [SerializeField] private byte maxPlayers = 2;

        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            Connect();
        }

        private void Connect()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = GameVersion;
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("[PhotonHandler] Connected to Master");
            PhotonNetwork.JoinRandomOrCreateRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("[PhotonHandler] Joining a random room failed");
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("[PhotonHandler] Created a new room");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("[PhotonHandler] Joined room");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("[PhotonHandler] Disconnected due to: {0}", cause);
        }
    }
}