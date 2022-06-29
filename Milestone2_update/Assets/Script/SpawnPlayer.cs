using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnPlayer : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public ChunkManager chunkManager;
    public float spawnHeight;
    public float minSpawnX;
    public float maxSpawnX;
    public float maxSpawnZ;
    public float minSpawnZ;
    void Start()
    {
        Vector3 randomPos = new Vector3(Random.Range(minSpawnX, maxSpawnX), spawnHeight, Random.Range(minSpawnZ, maxSpawnZ));
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity);

        chunkManager.playerCharacter = player.transform;
    }
}
