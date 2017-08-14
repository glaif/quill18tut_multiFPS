using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public float hitPoints = 100f;
    float currentHitPoints;

	// Use this for initialization
	void Start () {
        currentHitPoints = hitPoints;
	}

    [PunRPC]
    public void TakeDamage(float amt) {
        currentHitPoints -= amt;

        if (currentHitPoints <= 0) {
            Die();
        }
    }

    void Die() {
        if (GetComponent<PhotonView>().instantiationId == 0) {
            Destroy(gameObject);
            return;
        }
        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }
}
