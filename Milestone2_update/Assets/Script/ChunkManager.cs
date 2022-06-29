using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChunkManager : MonoBehaviour
{
    public bool showBorder = false;
    GameObject chunkHolder;
    string chunkHolderName = "Chunk Holder";
    public MeshGen m_generator;
    public Material material;

    public Transform playerCharacter;
    
    public List<Transform> playerList;

    Vector3Int lastPlayerChunkIndex;
    public int loadChunkRadius = 6;
    float SqDist => m_generator.chunkSize * m_generator.chunkSize * loadChunkRadius * loadChunkRadius;
    bool updateChunks = false;

    public Dictionary<Vector3Int, Chunk> chunkMapper; // stores all chunks loaded
    Queue<Chunk> chunkList;

    private void Awake()
    {
        Initialize();
        clearAllChunk();
    }

    private void Initialize()
    {
        chunkList = new Queue<Chunk> (); 
        chunkMapper = new Dictionary<Vector3Int, Chunk>();
        chunkMapper.Clear();
        if (chunkHolder == null)
        {
            if (GameObject.Find(chunkHolderName))
            {
                chunkHolder = GameObject.Find(chunkHolderName);
            } else
            {
                chunkHolder = new GameObject(chunkHolderName);
            }
        }

        disableAllChunk();

    }

    private Chunk spawnChunk(Vector3Int index, bool collide)
    {
        Chunk newChunk;
        if (chunkMapper.TryGetValue(index, out newChunk))
        {
            if (collide)
                newChunk.enableCollider();
            else
                newChunk.disableCollider();

            //Debug.Log("reached");
            if (!newChunk.gameObject.activeSelf)
            {
                newChunk.gameObject.SetActive(true);
                chunkList.Enqueue(newChunk);
            }
            
            return newChunk;
        }

        GameObject chunk = new GameObject($"Chunk {index}");
        chunk.transform.parent = chunkHolder.transform;
        newChunk = chunk.AddComponent<Chunk>();
        m_generator.initChunk(index, newChunk, material,collide);
        m_generator.GenerateNoise(newChunk);
        m_generator.GenerateMesh(newChunk);

        chunkMapper.Add(index, newChunk);
        chunkList.Enqueue(newChunk);

        return newChunk;
    }
    
    private void clearAllChunk()
    {
        var oldChunks = FindObjectsOfType<Chunk>(true);
        for (int i = oldChunks.Length - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(oldChunks[i].gameObject);
            else
                DestroyImmediate(oldChunks[i].gameObject);
        }
    }

    private void disableAllChunk()
    {
        var oldChunks = FindObjectsOfType<Chunk>();
        for (int i = oldChunks.Length - 1; i >= 0; i--)
        {
            oldChunks[i].DestroyOrDisable();
        }
    }

    private void Start()
    {
        Initialize();
        clearAllChunk();
    }

    private void Update()
    {
        checkChunkChange();
        //Debug.Log(lastPlayerChunkIndex);
        if (updateChunks)
        {
            unloadChunks();
            loadChunks();
        }

    }

    public void updateChunk(Vector3 pos, float rad)
    {
        pos = pos / m_generator.chunkSize;
        Vector3Int id = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
        int maxSizeChange = Mathf.CeilToInt(rad / m_generator.chunkSize);
        id = id - new Vector3Int(maxSizeChange, maxSizeChange, maxSizeChange);
        for (int x=0;x<maxSizeChange*2;x++ )
        {
            for (int y = 0;y<maxSizeChange*2;y++)
            {
                for (int z=0;z<maxSizeChange*2;z++)
                {
                    Vector3Int chunkId = id + new Vector3Int(x,y,z);
                    Chunk chunk;

                    if (chunkMapper.TryGetValue(chunkId, out chunk))
                    {
                        //Destroy(chunk.gameObject);
                        m_generator.changeChunk(chunk, pos, rad);
                    }
                }
            }
        }
    }
    private void checkChunkChange()
    {
        if (playerCharacter == null)
        {
            updateChunks = false;
            return;
        }


        float chunkSize = m_generator.chunkSize;
        Vector3Int chunkPos = new Vector3Int(
            Mathf.FloorToInt((playerCharacter.position.x - chunkSize / 2) / chunkSize),
            Mathf.FloorToInt((playerCharacter.position.y - chunkSize / 2) / chunkSize),
            Mathf.FloorToInt((playerCharacter.position.z - chunkSize / 2) / chunkSize)
         );

        if (lastPlayerChunkIndex == null || lastPlayerChunkIndex != chunkPos)
        {
            lastPlayerChunkIndex = chunkPos;
            updateChunks = true;
        }
    }

    private void unloadChunks()
    {
        //Debug.Log("reached!");
        Queue<Chunk> dummyList = new Queue<Chunk>();
        foreach (Chunk node in chunkList)
        {
            if (checkChunkInRadius(node))
            {
                dummyList.Enqueue(node);
            } else
            {
                node.DestroyOrDisable();
            }
        }
        chunkList = dummyList;
    }

    bool checkChunkInRadius(Vector3Int chunkIndex)
    {
        if ((playerCharacter.position - indexToWorldPos(chunkIndex)).sqrMagnitude > SqDist)
            return false;

        return true;
    }

    bool checkChunkInRadius(Chunk chunk)
    {
        return checkChunkInRadius(chunk.chunkIndex);
    }

    Vector3 indexToWorldPos(Vector3Int index)
    {
        Vector3 ret = new Vector3(index.x, index.y, index.z);
        ret = ret * m_generator.chunkSize - Vector3.one * (m_generator.chunkSize / 2);

        return ret;
    }

    private void loadChunks()
    {
        for (int i = -loadChunkRadius,j,k; i <= loadChunkRadius; ++i)
        {
            for (j = -loadChunkRadius; j <= loadChunkRadius; ++j)
            {
                for (k = -loadChunkRadius; k <= loadChunkRadius; ++k)
                {
                    Vector3Int chunkIndex = new Vector3Int(i, j, k) + lastPlayerChunkIndex;
                    if (checkChunkInRadius(chunkIndex))
                    {
                        spawnChunk(chunkIndex, true);
                    }
                }
            }

        }
    }


    private void OnValidate()
    {
        Initialize();
        checkChunkChange();
        //unloadChunks();
        //loadChunks();
        //Update();
    }

    private void OnDrawGizmos()
    {
        if (m_generator.changed)
        {
            m_generator.changed = false;
            OnValidate();
        }

    }
}
