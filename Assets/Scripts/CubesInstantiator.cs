using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

public class CubesInstantiator : MonoBehaviour
{
    public Vector3 cubesBasePos = new Vector3();
    public Vector3 mapPosOffset = new Vector3();
    public float gridScale = 8f;
    public float yScale = 1f;

    private readonly StreamReader _reader = new StreamReader("Assets/Resources/Data/landelevation_grid.json");

    // Start is called before the first frame update
    private void Start()
    {
        // Import JSON
        DataObjectArray dataObjectArray = JsonUtility.FromJson<DataObjectArray>(_reader.ReadToEnd());

        foreach (DataObject dataObject in dataObjectArray.data)
        {
            // Create Object
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var cubeRenderer = cube.GetComponent<Renderer>();
            cube.transform.parent = this.gameObject.transform;

            // Set Position and Scale
            cube.transform.position = new Vector3(dataObject.x, dataObject.y / 2 * yScale,
                dataObject.z) + cubesBasePos;
            cube.transform.localScale = new Vector3(1, dataObject.y * yScale, 1);

            // Set Color
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

        // Scale up this parent object
        gameObject.transform.localScale = new Vector3(gridScale, yScale, gridScale);

        // Move this parent object to the center of the map
        gameObject.transform.position += mapPosOffset;
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
