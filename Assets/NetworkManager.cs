using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {

    public bool offlineMode = false;
    public GameObject standbyCamera;
    SpawnSpot[] spawnSpots;
    GameObject guiGO;
    GameObject userNameFieldGO;
    InputField userNameField;
    List<string> chatMessages;
    int maxChatMessages = 6;
    Text chatTextObj = null;

    bool connecting = false;

    // Use this for initialization
    void Start () {
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        guiGO = GameObject.Find("_GUI Stuff_");
        PhotonNetwork.player.NickName = PlayerPrefs.GetString("Username", "PickAName");
        userNameFieldGO = GameObject.Find("UnameIField");
        if (userNameFieldGO == null) {
            Debug.Log("Error: could not find userNameFieldGO");
            return;
        }
        userNameField = userNameFieldGO.GetComponent<InputField>();
        if (userNameField == null) {
            Debug.Log("Error: could not find userNameField");
            return;
        }
        userNameField.text = PhotonNetwork.player.NickName;
        chatMessages = new List<string>();
    }

    private void OnDestroy() {
        PlayerPrefs.SetString("Username", PhotonNetwork.player.NickName);
    }

    public void AddChatMessage(string m) {
        GetComponent<PhotonView>().RPC("AddChatMessage_RPC", PhotonTargets.All, m);
    }

    [PunRPC]
    void AddChatMessage_RPC(string m) {
        while (chatMessages.Count >= maxChatMessages) {
            Debug.Log("Removing first chat message");
            chatMessages.RemoveAt(0);
        }
        Debug.Log("Adding chat message: " + m);
        chatMessages.Add(m);
    }

    void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        //if (PhotonNetwork.connected == false && connecting == false) { 
            // Do any setup stuff, prior to PUN connect
        //}

        if (PhotonNetwork.connected == true && connecting == false) {
            if (chatTextObj == null) {
                chatTextObj = GameObject.Find("ChatText").GetComponent<Text>();
            }
            StringBuilder sb = new StringBuilder();
            foreach (string msg in chatMessages) {
                sb.AppendLine(msg);
            }
            chatTextObj.text = sb.ToString();
        }
    }

    public void OnClickSinglePlayer() {
        guiGO.transform.Find("Menu").gameObject.SetActive(false);
        guiGO.transform.Find("Crosshairs").gameObject.SetActive(true);

        PhotonNetwork.offlineMode = true;
        connecting = true;
        OnJoinedLobby();
    }

    public void OnClickMultiPlayer() {
        PhotonNetwork.player.NickName = userNameField.text;
        Debug.Log("Username == " + PhotonNetwork.player.NickName);

        // Disable main menu & enable crosshairs HUD
        guiGO.transform.Find("Menu").gameObject.SetActive(false);
        guiGO.transform.Find("Crosshairs").gameObject.SetActive(true);

        GameObject.Find("Crosshairs").gameObject.SetActive(true);
        connecting = true;
        Connect();
    }

    void Connect() {
        PhotonNetwork.ConnectUsingSettings("dev_001");
    }

    // Bypassed if OFFLINE == true
    void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby");
        connecting = false;
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

        AddChatMessage("Spawning player: " + PhotonNetwork.player.NickName);
    }
}
