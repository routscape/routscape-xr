using System;
using Fusion;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;

namespace Pinning
{
    public class PersistentPinSpawnHandler : NetworkBehaviour
    {
        [SerializeField] private GameObject mapPin;
        private AbstractMap _mapManager;

        private void Start()
        {
            _mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
            if (_mapManager == null)
            {
                Debug.LogError("[PersistentPinSpawnHandler] No map found");
                throw new Exception("[PersistentPinSpawnHandler] No map found");
            }

            if (mapPin == null)
            {
                Debug.LogError("[PersistentPinSpawnHandler] No map pin found");
                throw new Exception("[PersistentPinSpawnHandler] No map pin found");
            }
        }

        public static event Action<string, Vector2d, GameObject> OnPinDrop;

        [Rpc]
        public void RpcSpawnPin(Vector3 position)
        {
            var latLong = position.ToVector2d();
            var pinName = "Pin - " + latLong.x + " " + latLong.y;
            _mapManager.VectorData.SpawnPrefabAtGeoLocation(mapPin, latLong, null, true,
                pinName);

            OnPinDrop.Invoke(pinName, latLong, null);
        }
    }
}