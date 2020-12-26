using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
		//Vector3 screenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//transform.position = new Vector3 (screenPoint.x, screenPoint.y, transform.position.z);
		//transform.position = screenPoint;
		transform.position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
    }
}
