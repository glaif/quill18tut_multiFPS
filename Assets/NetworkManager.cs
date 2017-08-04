using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    public Camera standbyCamera;
    SpawnSpot[] spawnSpots;

    // Use this for initialization
    void Start () {
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        Connect();
	}

    void Connect() {
        //PhotonNetwork.offlineMode = true;
        PhotonNetwork.ConnectUsingSettings("dev_001");
    }

    void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed() {
        Debug.Log("OnPhotonRandomJoinFailed");
        PhotonNetwork.CreateRoom(null);
    }

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
        standbyCamera.enabled = false;

        // NOTE: Must enable camera *before* controller, otherwise controller will have 
        // a null reference to the camera and will not get a new one
        myPlayerGO.transform.Find("FirstPersonCharacter").gameObject.SetActive(true);
        ((MonoBehaviour)myPlayerGO.GetComponent("FirstPersonController")).enabled = true;
    }
}
