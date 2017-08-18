using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : Photon.MonoBehaviour {

    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;

    Animator anim;

    bool gotFirstUpdate = false;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        if (anim == null) {
            Debug.LogError("Missing animator component");
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (photonView.isMine) {
            // Do nothing -- the character motor/input/etc. is moving us.
        } else {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
            // can lerp animation vars as well
        }
		
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // This is our player. We need to send our actual position to the network.

            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(anim.GetFloat("Speed"));
            stream.SendNext(anim.GetBool("Jumping"));
            stream.SendNext(anim.GetBool("Dancing"));
        } else {
            // This is someone else's player. We need to receive their position (as of
            // a few milliseconds ago), and update our version of that player.

            if (gotFirstUpdate == false) 
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            anim.SetFloat("Speed", (float)stream.ReceiveNext());
            anim.SetBool("Jumping", (bool)stream.ReceiveNext());
            anim.SetBool("Dancing", (bool)stream.ReceiveNext());
        }
    }
}
