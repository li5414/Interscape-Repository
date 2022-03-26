﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldingSlot : MonoBehaviour
{

	private Item item;
	private int holdingFrom;
	public Image image;

	public bool holdItem(Item item, int holdingFrom)
	{
		if (this.item != null) {
			Debug.Log("Refused to hold something because already holding something");
			return false;
		}

		this.item = item;
		this.holdingFrom = holdingFrom;
		image.enabled = true;
		image.sprite = item.icon;
		// if (item.iconColour != null)
		// 	image.color = item.iconColour.Value;
		// else {
		// 	image.color = Color.white;
		// }
		return true;
	}

	public Item removeHoldItem ()
	{
		Item returnItem = item;
		item = null;
		image.enabled = false;
		return returnItem;
	}

	public int getHoldingFrom() {
		return holdingFrom;
	}

	public Item getItem()
	{
		return item;
	}

	public bool isHolding()
	{
		return item != null;
	}

    // Start is called before the first frame update
    void Start()
    {
		item = null;
		image = gameObject.GetComponent<Image>();
		image.enabled = false;
    }
}
