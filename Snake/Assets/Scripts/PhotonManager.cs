using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [Header("Server settings")]
    [SerializeField] private string region;
    
    
    void Start()
    {
        Debug.Log("VAR");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion(region);
        Debug.Log("VAR1");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("You connected to "+ PhotonNetwork.CloudRegion);
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("You disconnected from server");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room is created with name " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room dont created :" + message);
    }
}
