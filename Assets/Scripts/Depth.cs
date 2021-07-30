using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depth : MonoBehaviour {

    private float Yposition;
    private float Xposition;
	
	void Update ()
    {
        Xposition = transform.position.x;
        Yposition = transform.position.y;
        transform.position = new Vector3(Xposition, Yposition, Yposition/100);
	}
}
