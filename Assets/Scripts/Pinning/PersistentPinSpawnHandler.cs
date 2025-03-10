using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;

namespace Pinning
{
    public class PersistentPinSpawnHandler : NetworkBehaviour
    {
        public static HashSet<string> PinsDropped = new();
        [SerializeField] private GameObject mapPin;
        private readonly Queue<Vector2d> _locationQueue = new();
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
            _mapManager.VectorData.SpawnPrefabAtGeoLocation(mapPin, latLong, PinDropCallback, true,
                pinName);
            _locationQueue.Enqueue(latLong);
        }

        private void PinDropCallback(List<GameObject> items)
        {
            var pin = items.ElementAt(0);
            var location = _locationQueue.Dequeue();
            var pinName = "Pin - " + location.x + " " + location.y;
            Debug.Log("Pin ID " + pinName);
            if (PinsDropped.Contains(pinName)) return;

            PinsDropped.Add(pinName);
            OnPinDrop.Invoke(pinName, location, items.ElementAt(0));
        }
    }
}