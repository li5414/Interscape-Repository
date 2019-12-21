using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flip : MonoBehaviour {

    bool right = true;
    Animator a;

    void Start()
    {
        a = GetComponent<Animator>();
    }

    void Flip()
    {
        right = !right;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");

        if (move > 0 && !right)
            Flip();
        else
        {
            if (move < 0 && right)
                Flip();
        }
    } 
} 
