using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropBob : MonoBehaviour
{
    float originalY;
    float offset;
    public float floatStrength;
    public float speed;

    void Start ()
    {
        this.originalY = this.transform.position.y;
        offset = transform.position.x;
    }

    void Update () {
        float newY = originalY + (Mathf.Sin((-Time.time * speed) + offset) * floatStrength + floatStrength);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
