using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSway : MonoBehaviour
{

	Animator anim;
	GameObject player;
	bool enteredFromRight;



	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		anim = GetComponent<Animator>();
	}


	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.tag.Equals ("Cursor"))
			return;

		if (player.transform.position.x > transform.position.x) {
			anim.Play ("shrub_left");
			enteredFromRight = true;
		} else {
			anim.Play ("shrub_right");
			enteredFromRight = false;

		}

	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (other.gameObject.tag.Equals ("Cursor"))
			return;

		if (enteredFromRight)
			anim.Play ("shrub_left_reverse");
		else {
			anim.Play ("shrub_right_reverse");
		}

	}
}
