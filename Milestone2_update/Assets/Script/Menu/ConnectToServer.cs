using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("lobby joined");
        SceneManager.LoadScene("Lobby"); 
    }

}
