using UnityEngine;
using UnityEngine.Rendering;
// source
// https://github.com/SebLague/Marching-Cubes/blob/master/Assets/Scripts/Chunk.cs
public class Chunk : MonoBehaviour
{
    public Vector3Int chunkIndex;

    public bool showNoise = false;
    public bool showBorder = false;

    [HideInInspector]
    public int triCount = 0;
    
    public float chunkSize;
    
    public float iso;
    public int chunkRes;

    [HideInInspector]
    public Mesh _mesh;
    MeshRenderer _meshRenderer;
    public MeshCollider _meshCollider;
    public MeshFilter _meshFilter;

    public ComputeBuffer voxel;

    // we couldnt get pure gpu render mesh to work with collisions
    //public GraphicsBuffer _indicesBuf;
    //public GraphicsBuffer _vertexBuf;

    public ComputeBuffer triangleBuf;

    public bool generateCollider;

    public Chunk(Vector3Int chunkIndex, float chunkSize)
    {
        this.chunkIndex = chunkIndex;
        this.chunkSize = chunkSize;
    }

    public void reEnable()
    {
        gameObject.SetActive(true);
    }

    public void DestroyOrDisable()
    {
        gameObject.SetActive(false);

        ////clearBuffer();
        //if (Application.isPlaying)
        //{
        //    //_mesh.Clear();
        //    gameObject.SetActive(false);
        //}
        //else
        //{
        //    //clearBuffer();
        //    //_mesh.Clear();
        //    gameObject.SetActive(false);
        //    //if (voxel != null)
        //    //    voxel.Dispose();
        //    //if (triangleBuf != null)
        //    //    triangleBuf.Release();

        //    ////Destroy(gameObject);
        //}
    }

    public void disableCollider()
    {
        generateCollider = false;
        if (_meshCollider != null)
            _meshCollider.enabled = false;
    }

    public void forceReloadCollision()
    {
        if (_meshCollider == null || !generateCollider)
            return;

        _meshCollider.enabled = false;
        _meshCollider.enabled = true;

    }

    public void enableCollider()
    {
        generateCollider = true;

        if (_meshCollider == null)
            _meshCollider = gameObject.AddComponent<MeshCollider>();
        else if (_meshCollider.sharedMesh != null)
        {
            _meshCollider.enabled = true;
        }
        else 
            _meshCollider.sharedMesh = _mesh;
    }

    public void clearBuffer()
    {
        if (triangleBuf != null)
        {
            triangleBuf.Release(); 
        }
    }


    public void AllocateMeshBuffer(int triangleCount)
    {
        triangleBuf = new ComputeBuffer(triangleCount, sizeof(float) * 3 * 2 * 3, ComputeBufferType.Append);
        triangleBuf.SetCounterValue(0);

        // currently pure gpu mesh generation is not working well with collisions

        //// We want GraphicsBuffer access as Raw (ByteAddress) buffers.
        //_mesh.indexBufferTarget |= GraphicsBuffer.Target.Raw;
        //_mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;

        //// Vertex position: float32 x 3
        //var vp = new VertexAttributeDescriptor
        //  (UnityEngine.Rendering.VertexAttribute.Position, VertexAttributeFormat.Float32, 3);

        //// Vertex normal: float32 x 3
        //var vn = new VertexAttributeDescriptor
        //  (UnityEngine.Rendering.VertexAttribute.Normal, VertexAttributeFormat.Float32, 3);

        //// Vertex/index buffer formats
        //_mesh.SetVertexBufferParams(vertexCount, vp, vn);
        //_mesh.SetIndexBufferParams(vertexCount, IndexFormat.UInt32);

        //// Submesh initialization
        //_mesh.SetSubMesh(0, new SubMeshDescriptor(0, vertexCount),
        //                 MeshUpdateFlags.DontRecalculateBounds);

        //// GraphicsBuffer references
        //_vertexBuf = _mesh.GetVertexBuffer(0);
        //_indicesBuf = _mesh.GetIndexBuffer();

    }

    public void SetupMesh(Material mat,bool generateCollider) 
    {
        this.generateCollider = generateCollider;

        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _meshRenderer = GetComponent<MeshRenderer>();

        if (_meshFilter == null)
            _meshFilter = gameObject.AddComponent<MeshFilter>();

        if (_meshRenderer == null)
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();

        if (_meshCollider == null && generateCollider)
            _meshCollider = gameObject.AddComponent<MeshCollider>();

        else if (!generateCollider && _meshCollider == null)
            DestroyImmediate(_meshCollider);
        _mesh = _meshFilter.sharedMesh; // get ref mesh
        if (_mesh == null)
        {
            _mesh = new Mesh();
            _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            _meshFilter.sharedMesh = _mesh;
            _mesh.name = string.Format("chunk{0}", chunkIndex);
        }

        _mesh.Clear();

        if (generateCollider)
        {
            if (_meshCollider.sharedMesh == null)
                _meshCollider.sharedMesh = _mesh;

            // force restart
            _meshCollider.enabled = false;
            _meshCollider.enabled = true;
        }
        _meshRenderer.material = mat;
    }

    private void OnDestroy()
    {
        clearBuffer();
        if (voxel != null)
            voxel.Release();
    }

    private void OnDrawGizmos()
    {
        if (showBorder && triCount > 0)
            Gizmos.DrawWireCube(new Vector3(chunkIndex.x, chunkIndex.y, chunkIndex.z) * chunkSize, Vector3.one * chunkSize);

        if (showNoise && voxel != null)
        {
            float[] voxelsVal = new float[(chunkRes+1) * (chunkRes + 1) * (chunkRes + 1)];
            voxel.GetData(voxelsVal);
            for (int x = 0, y, z, i = 0; x< chunkRes+1; ++x)
            {
                for (y = 0; y < chunkRes+1; ++y)
                {
                    for (z = 0; z < chunkRes+1; ++z, ++i)
                    {
                        if (voxelsVal[i] < iso) { 
                            Gizmos.DrawSphere(
                                (new Vector3(chunkIndex.x, chunkIndex.y, chunkIndex.z) * chunkRes
                                +
                                new Vector3(x, y, z)) * (chunkSize / (float)chunkRes) - Vector3.one * chunkSize / 2, iso - voxelsVal[i]);
                        }
                    }
                }
            }

        }
    }

}
