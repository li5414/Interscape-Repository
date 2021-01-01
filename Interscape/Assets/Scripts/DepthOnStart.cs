using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		float Xposition = transform.position.x;
		float Yposition = transform.position.y;
		transform.position = new Vector3 (Xposition, Yposition, Yposition / 100);
	}

   
}
