using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
	public ItemSlot [] itemSlots = new ItemSlot [10];


	void Start()
    {
		for (int i = 0; i < itemSlots.Length; i++) {
			itemSlots [i] = GetComponentsInChildren<ItemSlot> () [i];
		}
	}

}
