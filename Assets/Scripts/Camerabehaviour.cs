using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camerabehaviour : MonoBehaviour {

    public GameObject player;       //Public variable to reference the player game object
    private Vector3 offset;         //Private variable to store the offset
    public bool useOffset = true;

    // Use this for initialisation
    void Start() {
        // Calculate and store the offset value by getting the distance 
        // between the player's position and camera's position.
        offset = transform.position - player.transform.position;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate() {
        // Set the position of the camera's transform to be the same as the player's, 
        // but offset by the calculated offset distance.
        if (useOffset)
            transform.position = player.transform.position + offset;
        else
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }
}
