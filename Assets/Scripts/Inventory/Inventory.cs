using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	Item [] items = new Item [50];
	[SerializeField] GameObject itemsParent;
	[SerializeField] GameObject hotbarParent;
	ItemSlot [] itemSlots = new ItemSlot [50];
	[SerializeField] TextMeshProUGUI weightText;
	public float weight;
	private int count = 0;
	[SerializeField] Hotbar hotbar;

	public HoldingSlot holding;


	private void Start ()
	{
		int hotbarLength = 10;
		int i;
		for (i = 0; i < hotbarLength; i++) {
			itemSlots [i] = hotbarParent.GetComponentsInChildren<ItemSlot>() [i];
		}
		for (int j = i; j < items.Length; j++) {
			itemSlots[j] = itemsParent.GetComponentsInChildren<ItemSlot> () [j - i];
		}

		hotbar.Initialise ();
		RefreshUI();
		
		count = refreshCount ();

		// add items for debugging and testing
		AddItem (new Tool ("Axe", 300));
		AddItem (new Tool ("Pickaxe", 500));
		AddItem (new Tool ("Sword", 50));
		AddItem (new Tool ("Stone axe", 75));
		AddItem (new TerrainTool ("Hoe", 75));
		AddItem (new BuildableItem ("Mid Wood Floor"));
		gameObject.SetActive (false);
	}

	private int refreshCount()
	{
		int c = 0;
		for (int i = 0; i < items.Length; i++) {
			if (items [i] != null) {
				c++;
			}
		}
		return c;
	}

	private void RefreshUI ()
	{
		int i;
		for (i = 0; i < items.Length && i < itemSlots.Length; i++) {
			itemSlots[i].Item = items[i];
			if (i < 10) {
				hotbar.itemSlots [i].Item = items [i];
			}
		}
		RecalculateWeight ();
	}

	public void RecalculateWeight()
	{
		float sum = 0;
		for (int i = 0; i < items.Length && i < itemSlots.Length; i++) {
			if (items[i] != null) {
				sum += items [i].weight;
			}
		}
		weight = sum;
		weightText.text = "Weight: " + sum;
	}

	public bool AddItemAt(Item item, int slot)
	{
		if (IsFull ())
			return false;

		// check if empty then insert
		if (items[slot] == null) 
			items [slot] = item;
		 else 
			return false;
		

		RefreshUI ();
		return true;
	}

	public bool AddItem(Item item)
	{
		if (IsFull ())
			return false;

		// find next empty spot
		int slot = 0;
		bool isSlot = false;
		for (int i = 0; i < items.Length; i++) {
			if (items [i] == null) {
				slot = i;
				isSlot = true;
				break;
			}
		}

		if (isSlot)
			AddItemAt (item, slot);
		else
			return false;

		return true;
	}

	public bool RemoveItemAt(int slot)
	{
		if (items[slot] != null) {
			items [slot] = null;
			RefreshUI ();
			return true;
		}
		return false;
	}

	public Item getSelectedItem()
	{
		return hotbar.getSelectedItem ();
	}

	public bool RemoveSelectedItem ()
	{
		if (items [hotbar.getCurrentSelected()] != null) {
			items [hotbar.getCurrentSelected ()] = null;
			RefreshUI ();
			hotbar.updateSelectedUI();
			return true;
		}
		Debug.Log ("Tried to remove empty selected item");
		return false;
	}

	public bool IsFull()
	{
		return count >= itemSlots.Length;
	}

	public bool SwapToHolding(int slot)
	{
		if (holding.isHolding ()) {
			Debug.Log ("Tried to hold something when already holding");
			return false;
		}

		if (items[slot] == null) {
			Debug.Log ("Tried to hold nothing");
			return false;
		}

		if (holding.holdItem (items [slot], slot))
			RemoveItemAt (slot);
		else {
			Debug.Log ("Hold item refused :(");
			return false;
		}
		return true;
	}

	public bool CancelHold ()
	{
		if (!holding.isHolding()) {
			Debug.Log ("Tried to cancel holding nothing");
			return false;
		}
		SwapToInventory (holding.getHoldingFrom ());

		return true;
	}

	public bool SwapToInventory (int slot)
	{
		Item item = holding.removeHoldItem ();
		if (item == null) {
			Debug.Log ("Tried to put null item in inventory");
			return false;
		}

		if (items [slot] == null) {
			AddItemAt (item, slot);
			//Debug.Log ("put back in inventory");
		} else {
			swapWithHolding (item, slot, holding.getHoldingFrom());
			//Debug.Log ("Swapped item into place");
		}
			
		return true;
	}

	public bool DropHoldItem()
	{
		if (!holding.isHolding ()) {
			Debug.Log ("Tried to drop null item");
			return false;
		}

		ItemDrop.dropItem (holding.removeHoldItem ());

		return true;
	}

	public void swapWithHolding (Item item, int newSlot, int oldSlot)
	{

		Item oldItem = items [newSlot];
		RemoveItemAt(newSlot);
		AddItemAt (oldItem, oldSlot);
		//Debug.Log ("Addded " + oldItem.name + " at " + oldSlot);

		AddItemAt (item, newSlot);
		//Debug.Log ("Addded " + item.name + " at " + newSlot);
		//Debug.Log (oldItem.name);
	}

}
