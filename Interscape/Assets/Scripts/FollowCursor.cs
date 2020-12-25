using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
		transform.position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
    }
}
