using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    public Camera standbyCamera;

	// Use this for initialization
	void Start () {
        Connect();
	}

    void Connect() {
        PhotonNetwork.offlineMode = true;
        PhotonNetwork.ConnectUsingSettings("dev_001");
    }

    void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    void OnRandomJoinFailed() {
        Debug.Log("OnRandomJoinFailed");
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom");
        SpawnMyPlayer();
    }

    void SpawnMyPlayer() {
        PhotonNetwork.Instantiate("PlayerController", Vector3.zero, Quaternion.identity, 0);
        standbyCamera.enabled = false;
    }
}
