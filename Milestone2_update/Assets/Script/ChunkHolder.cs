using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkHolder : MonoBehaviour
{
    public Vector3Int chunkIndex;

    [HideInInspector]
    public Mesh mesh;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    bool generateCollider;

    public void DisableChunk()
    {
        mesh.Clear();
        gameObject.SetActive(false);
    }

    public void SetUp(Material mat, bool generateCollider)
    {
        this.generateCollider = generateCollider;

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();

        if (!this.generateCollider && meshCollider != null)
            DestroyImmediate(meshCollider);

        mesh = meshFilter.sharedMesh;
    }


}
