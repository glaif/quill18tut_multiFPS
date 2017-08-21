using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour {
    public GameObject sniperBulletFXPrefab;

    [PunRPC]
    void SniperBulletFX(Vector3 startPos, Vector3 endPos) {
        Debug.Log("SniperBulletFX");
        if (sniperBulletFXPrefab == null) {
            Debug.LogError("Missing sniperBulletFXPrefab");
            return;
        }

        GameObject sniperFX = (GameObject)Instantiate(sniperBulletFXPrefab, startPos, Quaternion.LookRotation(endPos - startPos));
        if (sniperFX == null) {
            Debug.LogError("Missing sniperFX");
            return;
        }

        LineRenderer lr = sniperFX.transform.Find("LineFX").GetComponent<LineRenderer>();
        if (lr == null) {
            Debug.LogError("Missing lr");
            return;
        }
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
    }

}
