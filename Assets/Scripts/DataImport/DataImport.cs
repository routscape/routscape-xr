using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataImport : MonoBehaviour
{
    [SerializeField] private Vector3 basePos = new Vector3();
    [SerializeField] private float yScale = 1f;
    
    [SerializeField] private Material waterMaterial;

	[SerializeField, Range(0, 100)]  
    private float heightThresholdPercentage = 0;
    
    private enum DataLayer {
	    landelevation,
	    floodrisk
    }
    
    [SerializeField] private DataLayer dataLayer;
    
    private readonly StreamReader _reader = new StreamReader("Assets/Resources/Data/landelevation.json");
	private List<GameObject> cubes = new List<GameObject>();
    private List<float> cubeHeights = new List<float>();
    
    // Start is called before the first frame update
    void Start()
	{
    	// Import JSON
    	DataObjectArray dataObjectArray = JsonUtility.FromJson<DataObjectArray>(_reader.ReadToEnd());

		// Scale the parent (this GameObject) based on map scale
    	this.transform.localScale = new Vector3(0.0402194308f, 0.0402194308f, 0.0402194308f);

    	foreach (DataObject dataObject in dataObjectArray.data)
    	{
        	// Create Object
        	GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        	var cubeRenderer = cube.GetComponent<Renderer>();
        	cube.transform.parent = this.gameObject.transform;

        	// Set Scale (apply map scale to maintain proportions)
        	cube.transform.localScale = new Vector3(1, dataObject.y, 1);

        	// Set Position (apply map scale to maintain proper spacing)
        	float adjustedHeight = (dataObject.y * yScale) / 2;  // Center the height
        	cube.transform.localPosition = new Vector3(
            	dataObject.x, 
            	adjustedHeight, 
            	dataObject.z
        	) + basePos;

			// For dynamic data
			cubes.Add(cube);
        	cubeHeights.Add(dataObject.y);
	        
	        // Set cube material
	        MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
	        if (renderer != null && waterMaterial != null)
	        {
		        renderer.material = waterMaterial;
	        }

        	// Set Color (elevation-based)
        	if (dataObject.y <= 2)
        	{
            	cubeRenderer.material.SetColor("_Color", new Color32(0xFF, 0xFF, 0xCC, 0xFF));
        	}
        	else if (dataObject.y <= 17)
        	{
            	cubeRenderer.material.SetColor("_Color", new Color32(0xC7, 0xE9, 0xB4, 0xFF));
        	}
        	else if (dataObject.y <= 35)
        	{
            	cubeRenderer.material.SetColor("_Color", new Color32(0x7F, 0xCD, 0xBB, 0xFF));
        	}
        	else if (dataObject.y <= 52)
        	{
            	cubeRenderer.material.SetColor("_Color", new Color32(0x41, 0xB6, 0xC4, 0xFF));
        	}
        	else
        	{
            	cubeRenderer.material.SetColor("_Color", new Color32(0x1D, 0x91, 0xC0, 0xFF));
        	}
    }
}
    
    // Update is called once per frame
    void Update()
    {
		// Loop through all cubes and toggle visibility based on the heightThreshold
        for (int i = 0; i < cubes.Count; i++)
        {
	        // Cube filtering based on height
            if (cubeHeights[i] >= heightThresholdPercentage)
            {
                cubes[i].SetActive(true);
            }
            else
            {
                cubes[i].SetActive(false);
            }
            
            // Cube binning based on flood risk hazard
            if (dataLayer == DataLayer.floodrisk)
            {
	            // if (cubes[i].localScale < 27.089)
	            // {
		           //  
	            // }
            }
        }
    }
}
