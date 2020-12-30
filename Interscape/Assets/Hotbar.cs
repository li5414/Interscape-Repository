using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
	public ItemSlot [] itemSlots = new ItemSlot [10];
	private ItemSlot selected;
	private SpriteRenderer playerHoldItem;
	public static Color selectedColor = new Color (1, 1, 1, 0.2f);
	private int prevSelected;
	private int currentSelected;
	private float minThreshold = 0.1f;

	public void Initialise ()
    {
		for (int i = 0; i < itemSlots.Length; i++) {
			itemSlots [i] = GetComponentsInChildren<ItemSlot> () [i];
		}
		selected = itemSlots [0];
		prevSelected = 0;
		currentSelected = 0;
		playerHoldItem = GameObject.FindGameObjectWithTag ("PlayerHoldItem").GetComponent<SpriteRenderer>();
		updateSelectedUI ();
	}

	public int getCurrentSelected()
	{
		return getCurrentSelected();
	}
	public Item getSelectedItem ()
	{
		return selected.Item;
	}

	private void Update ()
	{
		
		if (Input.mouseScrollDelta.y > minThreshold) {
			updateSelected (currentSelected, currentSelected + 1);
		}
		else if (Input.mouseScrollDelta.y < 0 - minThreshold) {
			updateSelected (currentSelected, currentSelected - 1);
		}

		else if (Input.inputString != "") {
			int number;
			bool is_a_number = Int32.TryParse (Input.inputString, out number);
			if (is_a_number && number > 0 && number < 10) {
				updateSelected (currentSelected, number - 1);
			} else if (is_a_number && number == 0) {
					updateSelected (currentSelected, 9);
			}
		}
	}

	private void updateSelected (int from, int to)
	{
		prevSelected = from;
		currentSelected = to;

		if (currentSelected > 9)
			currentSelected = 0;
		else if (currentSelected < 0)
			currentSelected = 9;

		updateSelectedUI ();
	}

	public void updateSelectedUI()
	{
		// update item sprites on player
		selected = itemSlots [currentSelected];
		if (selected.Item) {
			playerHoldItem.sprite = selected.Item.icon;
			playerHoldItem.enabled = true;
		} else {
			playerHoldItem.enabled = false;
		}

		//change colors of item slots
		itemSlots [prevSelected].GetComponent<Image> ().color = new Color (1, 1, 1, 0);
		selected.GetComponent<Image> ().color = selectedColor;
		
	}

}
