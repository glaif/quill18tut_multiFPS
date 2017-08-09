using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    bool PUN_OFFLINE = false;
    public GameObject standbyCamera;
    SpawnSpot[] spawnSpots;

    // Use this for initialization
    void Start () {
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        Connect();
	}

    void Connect() {
        if (PUN_OFFLINE) {
            PhotonNetwork.offlineMode = true;
            PhotonNetwork.CreateRoom(null);
            return;
        }
        PhotonNetwork.ConnectUsingSettings("dev_001");
    }

    void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
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

        // NOTE: Must enable camera *before* controller, otherwise controller will have 
        // a null reference to the camera and will not get a new one
        myPlayerGO.transform.Find("FirstPersonCharacter").gameObject.SetActive(true);
        ((MonoBehaviour)myPlayerGO.GetComponent("FirstPersonController")).enabled = true;
    }
}
