﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemRes : MonoBehaviour
{
	private static Sprite [] sprites = Resources.LoadAll<Sprite> ("Items/ItemsVer1");
	private static Sprite [] cobbleWallSprites = Resources.LoadAll<Sprite> ("Items/CobblestoneWallIcons");
	private static RuleTile lightCobbleWall = Resources.Load<RuleTile>("Sprites/Buildings/CobblestoneWallRuleTileWithGameObject");
	private static float defDur = 1;
	public static Dictionary<string, Item> ItemDict = new Dictionary<string, Item> {
				{ "Axe", new Tool ("Axe", sprites[2], "Good for killing trees (and anything really) \nbut quite heavy", 5, defDur, DamageType.Woodcutting, 300, 10, 0.5f)},
				{ "Branch", new Item ("Branch", sprites[9], "Tree arm", 2)},
				{ "Log", new Item ("Log", sprites[10], "Tree body", 6)},
				{ "Pickaxe", new Tool ("Pickaxe", sprites[1], "Good for breaking rocks", 3, defDur, DamageType.Rockbreaking, 300, 10, 0.5f)},
				{ "Stone", new Item ("Stone", sprites[8], "It's a stone.", 3)},
				{ "Stone axe", new Tool ("Stone axe", sprites[5], "A primitive axe", 3, defDur, DamageType.Woodcutting, 100, 5, 0.5f)},
				{ "Sword", new Tool ("Sword", sprites[0], "Useful for murder", 2, defDur, DamageType.Slicing, 300, 10, 0.5f)},
				{ "Light Cobblestone Wall", new BuildableItem("Light Cobblestone Wall", cobbleWallSprites[0], "A wall that looks like it's about to fall apart.", 6, lightCobbleWall)},
				{ "Cobblestone Wall", new BuildableItem("Cobblestone Wall", cobbleWallSprites[1], "A wall that looks like it's about to fall apart.", 6, lightCobbleWall)},
				{ "Dark Cobblestone Wall", new BuildableItem("Dark Cobblestone Wall", cobbleWallSprites[2], "A wall that looks like it's about to fall apart.", 6, lightCobbleWall)}
	};

	public static Item MakeItemCopy(string itemName) {
		Item item;
		ItemRes.ItemDict.TryGetValue (itemName, out item);

		if (item == null)
			Debug.Log("Could not find item");
		
		if (item is Tool) {
			return new Tool(itemName);
		} else if (item is BuildableItem) {
			return new BuildableItem(itemName);
		} 
		return new Item(itemName);
	}

}
