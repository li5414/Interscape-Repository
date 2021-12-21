using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsate : MonoBehaviour
{
    float originalScale;
    public float strength;
    public float speed;

    void Start ()
    {
        originalScale = transform.localScale.x;
    }

    void Update () {
        float newScale = originalScale + (Mathf.Sin(Time.time * speed) * strength);
        transform.localScale = new Vector3(newScale, newScale, 1);
    }
}
