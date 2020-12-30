using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursorInWorld : MonoBehaviour {
	Vector3 mousePosition;
	public Transform player;
	public Camera cam;

	
	void Update ()
	{
		mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
		mousePosition = cam.ScreenToWorldPoint (mousePosition);
		mousePosition.x = Mathf.FloorToInt (mousePosition.x) + 0.5f;
		mousePosition.y = Mathf.FloorToInt (mousePosition.y) + 0.5f;
		transform.position = mousePosition;

	}

}
