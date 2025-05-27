using System;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class PoiSpawner : MonoBehaviour
{
    [SerializeField] private GameObject mapPin;
    [SerializeField] private string csvFileName = "PH Evacuation Centers Location - Pasig"; 
    private AbstractMap _mapManager;

    [Serializable]
    public class POI
    {
        public string Barangay;
        public string EvacuationCenter;
        public Vector2 LatLong;
    }

    private List<POI> pois = new List<POI>();

    void Start()
    {
        _mapManager = GameObject.FindWithTag("mapbox map")?.GetComponent<AbstractMap>();
        if (_mapManager == null)
        {
            Debug.LogError("[PoiSpawner] No map found");
            throw new Exception("[PoiSpawner] No map found");
        }

        if (mapPin == null)
        {
            Debug.LogError("[PoiSpawner] No POI pin prefab assigned");
            throw new Exception("[PoiSpawner] No POI pin prefab assigned");
        }

        LoadPOIsFromCSV();
        SpawnPOIs();
    }

    void LoadPOIsFromCSV()
    {
        TextAsset csvData = Resources.Load<TextAsset>($"Data/{csvFileName}");
        if (csvData == null)
        {
            Debug.LogError($"[PoiSpawner] CSV file '{csvFileName}' not found in Resources/Data folder.");
            return;
        }

        string[] lines = csvData.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // skip header
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] parts = SplitCsvLine(line);
            if (parts.Length >= 3)
            {
                string[] coords = parts[2].Replace("\"", "").Split(',');
                if (coords.Length == 2 &&
                    float.TryParse(coords[0], out float lat) &&
                    float.TryParse(coords[1], out float lng))
                {
                    POI poi = new POI
                    {
                        Barangay = parts[0],
                        EvacuationCenter = parts[1],
                        LatLong = new Vector2(lat, lng)
                    };
                    pois.Add(poi);
                }
            }
        }
    }

    void SpawnPOIs()
    {        
        foreach (var poi in pois)
        {
            Vector2d latLong = new Vector2d(poi.LatLong.x, poi.LatLong.y);
            string pinName = $"{poi.EvacuationCenter} ({poi.Barangay})";
            
            _mapManager.VectorData.SpawnPrefabAtGeoLocation(
                mapPin,
                latLong,
                (instances) => {
                    foreach (var instance in instances)
                    {
                        var poiPin = instance.GetComponent<PoiPin>();
                        if (poiPin != null)
                        {
                            poiPin.SetLabel(pinName); // Set label on the spawned instance
                        }
                    }
                },
                true, // Use relative scale
                pinName
            );
            
            Debug.Log($"{pinName}: {latLong}");
        }
    }

    private string[] SplitCsvLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        string current = "";

        foreach (char c in line)
        {
            if (c == '"') inQuotes = !inQuotes;
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else current += c;
        }

        result.Add(current);
        return result.ToArray();
    }
}