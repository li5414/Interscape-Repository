using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	[SerializeField] List<Item> items;
	[SerializeField] Transform itemsParent;
	[SerializeField] ItemSlot [] itemSlots;



	private void Start()
	{
		RefreshUI();
		gameObject.SetActive (false);
	}

	private void OnValidate ()
	{
		if (itemsParent != null)
			itemSlots = itemsParent.GetComponentsInChildren<ItemSlot> ();

		RefreshUI ();
	}

	private void RefreshUI ()
	{
		int i;
		for (i = 0; i < items.Count && i < itemSlots.Length; i++) {
			itemSlots[i].Item = items[i];
		}

		// remaining empty slots
		for (; i < itemSlots.Length; i++) {
			itemSlots[i].Item = null;
		}
	}

	public bool AddItem(Item item)
	{
		if (IsFull ())
			return false;

		items.Add (item);
		RefreshUI ();
		return true;
	}

	public bool RemoveItem(Item item)
	{
		if (items.Remove(item)) {
			RefreshUI ();
			return true;
		}
		return false;
	}

	public bool IsFull()
	{
		return items.Count >= itemSlots.Length;
	}
}
