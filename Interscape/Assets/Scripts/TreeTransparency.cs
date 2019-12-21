using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTransparency : MonoBehaviour
{
	SpriteRenderer sRen;
	public GameObject player;
	private float Xposition;
	private float Yposition;
	private float Xpos;
	private float Ypos;


	void Start ()
	{
		sRen = GetComponent<SpriteRenderer>();
		player = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void Update()
    {
		Xpos = player.transform.position.x;
		Ypos = player.transform.position.y;
		Xposition = transform.position.x;
		Yposition = transform.position.y;
		Color newColour = new Color(1, 1, 1, 0.5f);

		if ((Ypos > Yposition - 2 & Ypos < Yposition + 7) &
			(Xpos > Xposition - 7 & Xpos < Xposition + 7)) {
			sRen.color = newColour;
		}
		else {
			newColour = new Color (1, 1, 1, 1);
			sRen.color = newColour;
		}

	}
}
