using UnityEngine;
using TMPro;
using Photon.Pun;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{

    public TMP_InputField roomId;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomId.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomId.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room joined!");

        PhotonNetwork.LoadLevel("Demo");
    }
}
