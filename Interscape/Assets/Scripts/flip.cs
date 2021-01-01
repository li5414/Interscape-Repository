using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flip : MonoBehaviour {

    public bool right = true;
	private Vector3 prevPos;

	private void Start ()
	{
		prevPos = transform.position;
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
		//float move = Input.GetAxis("Horizontal");
		float move = transform.position.x - prevPos.x;

        if (move > 0 && !right)
            Flip();
        else
        {
            if (move < 0 && right)
                Flip();
        }

		prevPos = transform.position;
    } 
} 
