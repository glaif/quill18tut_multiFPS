using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    /* This component is enabled for "my player" (i.e. the character beloning 
     * to the local client machine).
     */

    float speed = 10f;
    float jumpSpeed = 6f;
    Vector3 direction = Vector3.zero; // forward/back & left/right
    float verticalVelocity = -0.01f;

    CharacterController cc;
    Animator anim;

    // Use this for initialization
    void Start () {
        cc = GetComponent<CharacterController> ();
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        // Handle dancing
        if (Input.GetKey(KeyCode.H)) {
            anim.SetBool("Dancing", true);
            return;
        } else {
            anim.SetBool("Dancing", false);
        }

        // WASD forward/ back & left/right movement is stored in "direction"
        direction = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (direction.magnitude > 1f) {
            direction = direction.normalized;
        }

        anim.SetFloat("Speed", direction.magnitude);

        // Handle jumping
        if (cc.isGrounded && Input.GetButtonDown("Jump")) {
                verticalVelocity = jumpSpeed;
        }
        AdjustAimAngle();
    }

    void AdjustAimAngle() {
        Camera myCamera = this.GetComponentInChildren<Camera>();
        if (myCamera == null) {
            Debug.LogError("Null camera on player");
            return;
        }

        float AimAngle = 0;

        if (myCamera.transform.rotation.eulerAngles.x <= 90f) {
            // Looking down
            AimAngle = -myCamera.transform.rotation.eulerAngles.x;
        } else {
            AimAngle = 360 - myCamera.transform.rotation.eulerAngles.x;
        }

        anim.SetFloat("AimAngle", AimAngle);
    }

    // FixedUpdate is called once per physics loop
    // Do all movememt and other physics stuff here
    void FixedUpdate() {
        Vector3 dist = direction * speed * Time.deltaTime;

        if (cc.isGrounded && verticalVelocity < 0) {
            anim.SetBool("Jumping", false);
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        } else {
            if (Mathf.Abs(verticalVelocity) > jumpSpeed*0.75f) {
                anim.SetBool("Jumping", true);
            }
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
        
        dist.y = verticalVelocity * Time.deltaTime;

        cc.Move(dist);
    }
}
