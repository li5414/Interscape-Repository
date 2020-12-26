using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour
{
	public GameObject inventory;
	public GameObject itemTooltip;

	private void Update ()
	{
		if (Input.GetKeyDown(KeyCode.E)) {
			bool state = inventory.activeSelf;
			inventory.SetActive (!state);
			itemTooltip.SetActive (!state);
		}
	}
}
