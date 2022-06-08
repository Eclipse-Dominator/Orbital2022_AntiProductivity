using System.Collections.Generic;
using UnityEngine;

public class MeshGen : MonoBehaviour
{
    private static int renderThreads = 8;

    public float chunkSize;
    public int chunkRes;
    public float isoValue;

    public bool changed = true;

    public ComputeShader marcher;
    struct Vertex
    {
        public Vector3 pos;
        public Vector3 normal;
    }
    struct Triangle
    {
        public Vertex v1;
        public Vertex v2;
        public Vertex v3;
    }
    int maxTriangles => (chunkRes)* (chunkRes )* (chunkRes )*5;

    // one extra to account for joining meshes
    int noiseSize => (chunkRes+1) * (chunkRes+1) * (chunkRes+1); 

    public NoiseGenerator noiseGenerator;


    public void GenerateNoise(Chunk chunk)
    {
        noiseGenerator.chunkSize = chunkSize;
        noiseGenerator.numOfPoints = (int)chunkRes;
        if (chunk.voxel != null)
            chunk.voxel.Dispose();

        chunk.voxel = new ComputeBuffer(noiseSize, sizeof(float));
        
        noiseGenerator.GenerateNoise(chunk.chunkIndex, chunk.voxel);
    }

    public void initChunk(Vector3Int chunkIndex, Chunk chunk, Material mat,bool collider)
    {
        chunk.chunkSize = chunkSize;
        chunk.chunkIndex = chunkIndex;
        chunk.chunkRes = (int)chunkRes;
        chunk.iso = isoValue;
        chunk.SetupMesh(mat, collider);
        chunk.AllocateMeshBuffer((int)maxTriangles * 3);
    }

    public Chunk makeChunk(Vector3Int chunkIndex, Material mat, bool collide)
    {
        Chunk ret = new Chunk(chunkIndex, chunkSize);
        ret.chunkRes = (int)chunkRes;
        ret.iso = isoValue;
        ret.SetupMesh(mat, collide);
        ret.AllocateMeshBuffer((int)maxTriangles);

        return ret; 
    }
    private void OnDrawGizmos()
    {
        if (noiseGenerator.changed)
        {
            noiseGenerator.changed = false;
            OnValidate();
        }

    }

    private void OnValidate()
    {
        changed = true;
    }

    public void GenerateMesh(Chunk chunk)
    {
        ComputeBuffer voxelBuf = chunk.voxel;
        
        int numThread = Mathf.CeilToInt((float)chunkRes / renderThreads);
        //ComputeBuffer CountBuf = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Counter);
        ComputeBuffer CountBuf = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        //ComputeBuffer triBuffer = new ComputeBuffer(maxTriangles, sizeof(float) * 3 * 3, ComputeBufferType.Append);
        CountBuf.SetCounterValue(0);
        //triBuffer.SetCounterValue(0);

        //marcher.SetBuffer(0, "trianglesBuf", triBuffer);
        marcher.SetBuffer(0, "voxel", voxelBuf);

        //marcher.SetBuffer(0, "VertexBuffer", chunk._vertexBuf);
        //marcher.SetBuffer(0, "IndexBuffer", chunk._indicesBuf);
        marcher.SetBuffer(0, "trianglesBuf", chunk.triangleBuf);
        marcher.SetBuffer(0, "counter", CountBuf);

        
        marcher.SetInt("chunkRes", chunkRes);
        marcher.SetInt("MaxTriangle", (int)maxTriangles);
        marcher.SetInts("chunkIndex", new int[] { chunk.chunkIndex.x, chunk.chunkIndex.y, chunk.chunkIndex.z });

        marcher.SetFloat("chunkSize", chunkSize);
        marcher.SetFloat("isovalue", isoValue);

        marcher.Dispatch(0, numThread, numThread, numThread);

        ComputeBuffer.CopyCount(chunk.triangleBuf, CountBuf, 0);
        int[] c = { 0 };
        CountBuf.GetData(c);
        int triTot = c[0];
        //Debug.Log(triTot * 3);
        Triangle[] tris = new Triangle[triTot];
        chunk.triangleBuf.GetData(tris, 0, 0, triTot);

        Vector3[] vertex = new Vector3[triTot * 3];
        Vector3[] normal = new Vector3[triTot * 3];
        int[] index = new int[triTot * 3];

        for (int i = 0; i < triTot; i++)
        {
            Triangle tri = tris[i];

            vertex[i * 3 + 0] = tri.v1.pos;
            vertex[i * 3 + 1] = tri.v2.pos;
            vertex[i * 3 + 2] = tri.v3.pos;

            normal[i * 3 + 0] = tri.v1.normal;
            normal[i * 3 + 1] = tri.v2.normal;
            normal[i * 3 + 2] = tri.v3.normal;

            index[i * 3 + 0] = i * 3 + 0;
            index[i * 3 + 1] = i * 3 + 1;
            index[i * 3 + 2] = i * 3 + 2;
        }
        chunk._mesh.Clear();
        chunk._mesh.vertices = vertex;
        chunk._mesh.normals = normal;
        chunk._mesh.triangles =index;
        chunk.triCount = triTot;
        chunk.forceReloadCollision();


        CountBuf.Release();
        chunk.clearBuffer();
    }
}
