using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropSettings : MonoBehaviour
{

	public static GameObject itemDropPrefab;
	public static GameObject player;
	public static Inventory inventory;
	public static GameObject parent;

	// Start is called before the first frame update
	void Start()
    {
		player = GameObject.FindGameObjectWithTag ("Player");
		inventory = GameObject.FindGameObjectWithTag ("Inventory").GetComponent<Inventory> ();
		parent = GameObject.FindGameObjectWithTag ("ItemDropParent");
		itemDropPrefab = Resources.Load<GameObject> ("Items/ItemDrop");
	}

}
