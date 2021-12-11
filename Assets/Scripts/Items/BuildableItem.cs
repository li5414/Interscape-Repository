using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableItem : Item
{
    public BuildableItem (string itemName) : base (itemName)
	{
	}
    public BuildableItem (string itemName, Sprite icon, string description, float weight) 
    : base (itemName, icon, description, weight)
	{
	}

    public void BuildItemInWorld() {
        Debug.Log("hehe im building");
    }
}
