using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour
{
	public float bobDistance = 0.02f;
	public float bobSpeed = 4f;
	float offset = 0;

    public void Start()
    {
        offset = Random.Range(0, 2 * Mathf.PI);
    }

    // Update is called once per frame
    void Update()
    {
		this.transform.Translate (new Vector3 (0.0f, bobDistance * Mathf.Sin (bobSpeed * Time.time + offset)), 0.0f);
	}

}
