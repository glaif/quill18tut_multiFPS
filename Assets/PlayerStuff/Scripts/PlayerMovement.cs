using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    /* This component is enabled for "my player" (i.e. the character beloning 
     * to the local client machine).
     */

    float speed = 10f;
    Vector3 direction = Vector3.zero;

    CharacterController cc;

    // Use this for initialization
    void Start () {
        cc = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
        direction = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
	}

    // FixedUpdate is called once per physics loop
    // Do all movememt and other physics stuff here
    void FixedUpdate() {
        cc.SimpleMove(direction * speed);
    }
}
