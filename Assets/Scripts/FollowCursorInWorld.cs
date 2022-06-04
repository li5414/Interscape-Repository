using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursorInWorld : MonoBehaviour {
    Vector3 mousePosition;
    public Transform player;
    public Camera cam;
    private float zPos;

    public bool snapToTiles = true;

    void Start() {
        zPos = transform.position.z;
    }
    void FixedUpdate() {
        mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zPos);
        mousePosition = cam.ScreenToWorldPoint(mousePosition);
        if (snapToTiles) {
            mousePosition.x = Mathf.FloorToInt(mousePosition.x) + 0.5f;
            mousePosition.y = Mathf.FloorToInt(mousePosition.y) + 0.5f;
        }
        mousePosition.z = zPos;
        transform.position = mousePosition;
    }
}
