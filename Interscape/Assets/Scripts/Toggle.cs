using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour
{
	public GameObject inventory;
	public GameObject itemTooltip;
	public GameObject hotbar;
	public GameObject cursor;

	private bool hideHotbar;

	private void Update ()
	{
		if (Input.GetKeyDown (KeyCode.F1) && inventory.activeSelf == false) {
			hideHotbar = !hideHotbar;
			hotbar.SetActive (!hideHotbar);
		}

		if (Input.GetKeyDown(KeyCode.E)) {
			bool state = inventory.activeSelf;
			if (state)
				itemTooltip.SetActive (false);
			inventory.SetActive (!state);

			if (!hideHotbar) {
				hotbar.SetActive (state);
			}
			cursor.SetActive (state);
		}

		
	}
}
