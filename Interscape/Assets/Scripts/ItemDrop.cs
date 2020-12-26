using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDrop : MonoBehaviour
{
	[SerializeField] Item item;
	private SpriteRenderer image;
	private static GameObject player;
	private static float dropRadius = 0.7f;
	private static GameObject itemDropPrefab;
	private static GameObject parent;


	// Start is called before the first frame update
	void Awake ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");


		image = GetComponentsInChildren<SpriteRenderer> ()[1];
	}


	/*public static bool dropItemAt (Item item, Vector3 pos)
	{
		if (item == null)
			return false;

		GameObject obj = Instantiate (itemDropPrefab, pos, Quaternion.identity);
		ItemDrop drop = obj.GetComponent<ItemDrop> ();
		drop.item = item;
		drop.image.sprite = item.icon;
		return true;
	}*/

	public static bool dropItem (Item item)
	{
		if (item == null)
			return false;

		if (!itemDropPrefab) {
			itemDropPrefab = Resources.Load<GameObject> ("Items/ItemDrop");
		}

		if (!parent) {
			parent = GameObject.FindGameObjectWithTag ("ItemDropParent");
		}

		GameObject obj = Instantiate (itemDropPrefab, parent.transform);
		ItemDrop drop = obj.GetComponent<ItemDrop> ();
		drop.item = item;
		drop.image.sprite = item.icon;

		Vector2 pos = Random.insideUnitCircle * dropRadius;
		obj.transform.position = new Vector3 (player.transform.position.x + pos.x, player.transform.position.y + pos.y, player.transform.position.z);

		return true;
	}

}
