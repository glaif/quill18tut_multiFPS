using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {

    public float fireRate = 0.5f;
    float cooldown = 0f;
    public float damage = 25f;
    FXManager fxManager;

    private void Start() {
        fxManager = GameObject.FindObjectOfType<FXManager>();

        if (fxManager == null) {
            Debug.LogError("Couldn't find an FXManage object");
        }
    }

    // Update is called once per frame
    void Update () {
        cooldown -= Time.deltaTime;

        if (Input.GetButton("Fire1")) {
            Fire();
        }
	}

    void Fire() {
        if (cooldown > 0)
            return;

        Debug.Log("Firing our gun!");

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Transform hitTransform;
        Vector3 hitPoint;

        hitTransform = FindClosestHitObject(ray, out hitPoint);

        if (hitTransform != null) {
            // We hit something
            Debug.Log("We hit: " + hitTransform.name);

            Health h = hitTransform.GetComponent<Health>();

            while (h == null && hitTransform.parent) {
                hitTransform = hitTransform.parent;
                h = hitTransform.GetComponent<Health>();
            }
            // Once we reach here, hitTransform may not be the same one we started with

            if (h != null) {
                // This next line is the equivalent of calling
                //      h.TakeDamage(damage);
                // Except over the network
                PhotonView pv = h.GetComponent<PhotonView>();
                if (pv == null)
                    Debug.LogError("Error: Null PhotonView object");

                pv.RPC("TakeDamage", PhotonTargets.AllBuffered, damage);
            }

            // Do bullet hit FX
            if (fxManager != null) {
                fxManager.GetComponent<PhotonView>().RPC("SniperBulletFX", PhotonTargets.All, Camera.main.transform.position, hitPoint);
            }
        } else {
            // We did not hit anything, still need bullet FX
            if (fxManager != null) {
                hitPoint = Camera.main.transform.position + (Camera.main.transform.forward * 100f);
                fxManager.GetComponent<PhotonView>().RPC("SniperBulletFX", PhotonTargets.All, Camera.main.transform.position, hitPoint);
            }
        }
        cooldown = fireRate;
    }

    Transform FindClosestHitObject(Ray ray, out Vector3 hitPoint) {
        RaycastHit[] hits = Physics.RaycastAll(ray);

        Transform closestHit = null;
        float distance = 0f;
        hitPoint = Vector3.zero;

        foreach (RaycastHit hit in hits) {
            if (hit.transform != this.transform && (closestHit == null || hit.distance < distance)) {
                // We have hit something that is:
                // a) not us
                // b) the first thing we hit (that is not us)
                // c) or, if not b, is at least closer than the previous closest thing

                closestHit = hit.transform;
                distance = hit.distance;
                hitPoint = hit.point;
            }
        }

        // Closest hit is now either still null (i.e. we hit nothing) 
        // OR it contains the closest thing that is a valid thing to hit
        return closestHit;
    }
}
