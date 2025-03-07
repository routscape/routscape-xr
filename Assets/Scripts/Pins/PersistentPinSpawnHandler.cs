using System;
using Fusion;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using UnityEngine;

namespace Pins
{
    public class PersistentPinSpawnHandler : NetworkBehaviour
    {
        private AbstractMap _mapManager;
        public GameObject MapPin { private get; set; }

        private void Start()
        {
            _mapManager = GameObject.FindWithTag("mapbox map").GetComponent<AbstractMap>();
            if (_mapManager == null)
            {
                Debug.LogError("[PersistentPinSpawnHandler] No map found");
                throw new Exception("[PersistentPinSpawnHandler] No map found");
            }
        }

        [Rpc]
        public void RpcSpawnPin(Vector3 position)
        {
            if (MapPin == null)
            {
                Debug.LogError("[PersistentPinSpawnHandler] No MapPin prefab set");
                throw new Exception("[PersistentPinSpawnHandler] No MapPin prefab set");
            }

            _mapManager.VectorData.SpawnPrefabAtGeoLocation(MapPin, position.ToVector2d(), null, true, "Pin");
        }
    }
}