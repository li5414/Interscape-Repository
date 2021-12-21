using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BuildableItem : Item
{
    public RuleTile buildingReference;

    public BuildableItem (string itemName) : base (itemName)
	{
        Item item;
		ItemRes.ItemDict.TryGetValue (itemName, out item);
        
		if (item != null && item is BuildableItem) {
			this.buildingReference = ((BuildableItem)item).buildingReference;
		}
	}
    public BuildableItem (string itemName, Sprite icon, string description, float weight, RuleTile buildingReference) 
    : base (itemName, icon, description, weight)
	{
        this.buildingReference = buildingReference;
	}

    public void BuildItemAt(Vector3 worldPos, Inventory inventory) {
        Tilemap tilemap = GameObject.FindWithTag("RuleTilemap").GetComponent<Tilemap>();
        if (buildingReference != null) {
            tilemap.SetTile(tilemap.WorldToCell(worldPos), buildingReference);
            inventory.RemoveSelectedItem();
        }
        else {
            Debug.Log("The building object is null");
        }
    }
}
