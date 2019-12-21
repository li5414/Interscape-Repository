using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour
{
	public GameObject inventory;

	private void Update ()
	{
		if (Input.GetKeyDown(KeyCode.E)) {
			inventory.SetActive (!inventory.activeSelf);
		}
	}
}
