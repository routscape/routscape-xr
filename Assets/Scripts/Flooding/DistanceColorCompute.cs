using System.Runtime.InteropServices;
using UnityEngine;

namespace Flooding
{
    public class DistanceColorCompute : MonoBehaviour
    {
        public ComputeShader distanceShader;
        public MeshFilter referenceMeshFilter; // The other mesh to compare against
        public float maxDistance = 10.0f;
        public Color closeColor = Color.blue;
        public Color farColor = Color.red;

        private bool meshesUpdated;
        private MeshFilter meshFilter;

        // Buffers for mesh data
        private ComputeBuffer meshVertexBuffer;
        private ComputeBuffer referenceMeshVertexBuffer;

        private RenderTexture resultTexture;
        private Renderer targetRenderer;

        private void Start()
        {
            targetRenderer = GetComponent<Renderer>();
            meshFilter = GetComponent<MeshFilter>();
            InitializeTexture();
            InitializeBuffers();
            UpdateShader();
        }

        private void Update()
        {
            if (meshesUpdated)
            {
                UpdateShader();
                meshesUpdated = false;
            }
        }

        private void OnDestroy()
        {
            // Clean up buffers
            ReleaseBuffers();
        }

        private void InitializeTexture()
        {
            resultTexture = new RenderTexture(512, 512, 0);
            resultTexture.enableRandomWrite = true;
            resultTexture.Create();

            targetRenderer.material.SetTexture("_MainTex", resultTexture);
        }

        private void InitializeBuffers()
        {
            if (meshFilter != null && meshFilter.sharedMesh != null) UpdateMesh(meshFilter.sharedMesh);

            if (referenceMeshFilter != null && referenceMeshFilter.sharedMesh != null)
                UpdateReferenceMesh(referenceMeshFilter.sharedMesh);
        }

        private void ReleaseBuffers()
        {
            if (meshVertexBuffer != null)
            {
                meshVertexBuffer.Release();
                meshVertexBuffer = null;
            }

            if (referenceMeshVertexBuffer != null)
            {
                referenceMeshVertexBuffer.Release();
                referenceMeshVertexBuffer = null;
            }
        }

        private void UpdateShader()
        {
            if (meshVertexBuffer == null || referenceMeshVertexBuffer == null)
                return;

            var kernelIndex = distanceShader.FindKernel("CSMain");

            // Set buffers
            distanceShader.SetBuffer(kernelIndex, "MeshVertices", meshVertexBuffer);
            distanceShader.SetBuffer(kernelIndex, "ReferenceMeshVertices", referenceMeshVertexBuffer);

            // Set vertex counts
            distanceShader.SetInt("MeshVertexCount", meshFilter.sharedMesh.vertexCount);
            distanceShader.SetInt("ReferenceMeshVertexCount", referenceMeshFilter.sharedMesh.vertexCount);

            // Set transform matrices
            distanceShader.SetMatrix("LocalToWorld", transform.localToWorldMatrix);
            distanceShader.SetMatrix("ReferenceLocalToWorld", referenceMeshFilter.transform.localToWorldMatrix);

            // Set other parameters
            distanceShader.SetFloat("MaxDistance", maxDistance);
            distanceShader.SetVector("CloseColor", closeColor);
            distanceShader.SetVector("FarColor", farColor);

            distanceShader.SetTexture(kernelIndex, "Result", resultTexture);

            // Dispatch
            distanceShader.Dispatch(kernelIndex, resultTexture.width / 8, resultTexture.height / 8, 1);
        }

        // Public method to update the current mesh
        public void UpdateMesh(Mesh newMesh)
        {
            if (newMesh == null)
                return;

            // Release previous buffer if it exists
            if (meshVertexBuffer != null)
            {
                meshVertexBuffer.Release();
                meshVertexBuffer = null;
            }

            // Create new buffer with the updated vertices
            var vertices = newMesh.vertices;
            meshVertexBuffer = new ComputeBuffer(vertices.Length, Marshal.SizeOf<Vector3>());
            meshVertexBuffer.SetData(vertices);

            meshesUpdated = true;
        }

        // Public method to update the reference mesh
        public void UpdateReferenceMesh(Mesh newReferenceMesh)
        {
            if (newReferenceMesh == null)
                return;

            // Release previous buffer if it exists
            if (referenceMeshVertexBuffer != null)
            {
                referenceMeshVertexBuffer.Release();
                referenceMeshVertexBuffer = null;
            }

            // Create new buffer with the updated vertices
            var refVertices = newReferenceMesh.vertices;
            referenceMeshVertexBuffer = new ComputeBuffer(refVertices.Length, Marshal.SizeOf<Vector3>());
            referenceMeshVertexBuffer.SetData(refVertices);

            meshesUpdated = true;
        }
    }
}