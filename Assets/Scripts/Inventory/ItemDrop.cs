using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrop : MonoBehaviour
{
	[SerializeField] Item item;
	private SpriteRenderer image;
	
	void Awake ()
	{
		image = GetComponentsInChildren<SpriteRenderer> ()[1];
	}

	public static bool dropItemAt (Item item, Vector3 at, float dropRadius)
	{
		if (item == null)
			return false;

		GameObject obj = Instantiate (ItemDropSettings.itemDropPrefab, ItemDropSettings.parent.transform);
		ItemDrop drop = obj.GetComponent<ItemDrop> ();
		drop.item = item;
		drop.image.sprite = item.icon;
		if (item.iconColour != null)
			drop.image.color = item.iconColour.Value;

		Vector2 pos = Random.insideUnitCircle * dropRadius;
		obj.transform.position = new Vector3 (at.x + pos.x, at.y + pos.y, at.z);

		return true;
	}

	public static bool dropItem (Item item)
	{
		if (item == null)
			return false;

		GameObject obj = Instantiate (ItemDropSettings.itemDropPrefab, ItemDropSettings.parent.transform);
		ItemDrop drop = obj.GetComponent<ItemDrop> ();
		drop.item = item;
		drop.image.sprite = item.icon;
		if (item.iconColour != null)
			drop.image.color = item.iconColour.Value;

		Vector2 pos = Random.insideUnitCircle * 0.7f; // drop radius
		obj.transform.position = new Vector3 (ItemDropSettings.player.transform.position.x + pos.x, ItemDropSettings.player.transform.position.y + pos.y, ItemDropSettings.player.transform.position.z);

		return true;
	}

	public bool pickupItem (ItemDrop item)
	{
		if (ItemDropSettings.inventory.AddItem(item.item)) {
			Destroy (item.gameObject);
			return true;
		}
		return false;
	}

	public void OnMouseOver ()
	{
		if (Input.GetMouseButton (1)) {
			pickupItem (this);
		}
	}
}
