using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Laucher : MonoBehaviourPunCallbacks
{

    public PhotonView playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        // connect to server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to master");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room");
        PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
    }
}
