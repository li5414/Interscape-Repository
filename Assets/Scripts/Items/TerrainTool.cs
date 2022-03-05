using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainTool : Tool
{
	RuleTile tileReference;
    Tilemap tilemapReference;

	public TerrainTool (string itemName) : base (itemName)
	{
		Item item;
		ItemRes.ItemDict.TryGetValue (itemName, out item);

		if (item != null && item is TerrainTool) {
			// this.durability = ((TerrainTool)item).durability;
			// this.damageType = ((TerrainTool)item).damageType;
			// this.baseDurability = ((TerrainTool)item).baseDurability;
			// this.baseDamage = ((TerrainTool)item).baseDamage;
			// this.coolDown = ((TerrainTool)item).coolDown;
            this.tileReference = ((TerrainTool)item).tileReference;
            this.tilemapReference = ((TerrainTool)item).tilemapReference;
			refreshQualities ();
		} else {
            Debug.Log("Error finding TerrainTool: " + itemName + " in database");
        }
	}

	public TerrainTool (string itemName, float durability) : base (itemName, durability)
	{
		Item item;
		ItemRes.ItemDict.TryGetValue (itemName, out item);

		if (item != null && item is TerrainTool) {
			// this.durability = durability;
			// this.damageType = ((TerrainTool)item).damageType;
			// this.baseDurability = ((TerrainTool)item).baseDurability;
			// this.baseDamage = ((TerrainTool)item).baseDamage;
			// this.coolDown = ((TerrainTool)item).coolDown;
            this.tileReference = ((TerrainTool)item).tileReference;
            this.tilemapReference = ((TerrainTool)item).tilemapReference;
			refreshQualities ();
		} else {
            Debug.Log("Error finding TerrainTool: " + itemName + " in database");
        }
	}

	public TerrainTool (string itemName, Sprite icon, string description, float weight,
		float durability, DamageType damageType, float baseDurability,
		float baseDamage, float coolDown, RuleTile tileReference, 
        string tilemapReference) : base (itemName, icon, description, weight, 
        durability, damageType, baseDurability, baseDamage, coolDown)
    {
        this.tileReference = tileReference;
        this.tilemapReference = GameObject.FindWithTag(tilemapReference).GetComponent<Tilemap>();
		refreshQualities ();
	}

    public void BuildItemAt(Vector3 worldPos) {
        if (tileReference != null) {
            tilemapReference.SetTile(tilemapReference.WorldToCell(worldPos), tileReference);
        }
        else {
            Debug.Log("The tileReference object is null");
        }
    }
}
