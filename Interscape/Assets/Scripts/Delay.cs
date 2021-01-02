using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DelayReached : UnityEvent { }

public class Delay : MonoBehaviour
{
    public float delayInterval;
    public float time = 0;
	public bool loop = true;
	public DelayReached delayReached;
	
	private bool hasInvoked = false;


    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
		if (time > delayInterval) {
			if (loop == true || hasInvoked == false) {
				delayReached.Invoke ();
				hasInvoked = true;
				time = 0;
			}
        }
    }
}
