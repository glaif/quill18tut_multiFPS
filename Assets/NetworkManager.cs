using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    public bool offlineMode = false;
    public GameObject standbyCamera;
    SpawnSpot[] spawnSpots;

    bool connecting = false;

    // Use this for initialization
    void Start () {
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
	}

    void Connect() {
        PhotonNetwork.ConnectUsingSettings("dev_001");
    }

    void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        if (PhotonNetwork.connected == false && connecting == false) {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Single Player")) {
                PhotonNetwork.offlineMode = true;
                connecting = true;
                OnJoinedLobby();
            }
            if (GUILayout.Button("Multi-Player")) {
                connecting = true;
                Connect();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }

    // Bypassed if OFFLINE == true
    void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    // Bypassed if OFFLINE == true
    void OnPhotonRandomJoinFailed() {
        Debug.Log("OnPhotonRandomJoinFailed");
        PhotonNetwork.CreateRoom(null);
    }

    // Reached no matter what OFFLINE is (true | false)
    void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom");
        SpawnMyPlayer();
    }

    void SpawnMyPlayer() {
        if (spawnSpots == null) {
            Debug.LogError("NULL array of spawn spots");
            return;
        }
        SpawnSpot mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];
        GameObject myPlayerGO = PhotonNetwork.Instantiate("PlayerController", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
        standbyCamera.SetActive(false);

        myPlayerGO.transform.Find("FirstPersonCharacter").gameObject.SetActive(true);
        ((MonoBehaviour)myPlayerGO.GetComponent("MouseLook")).enabled = true;
        ((MonoBehaviour)myPlayerGO.GetComponent("PlayerMovement")).enabled = true;
        ((MonoBehaviour)myPlayerGO.GetComponent("PlayerShooting")).enabled = true;
    }
}
