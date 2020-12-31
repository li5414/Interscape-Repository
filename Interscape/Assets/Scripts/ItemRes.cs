using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRes : MonoBehaviour
{
	//public static Dictionary<string, Item> ItemDict = new Dictionary<string, Item> ();
	private static Sprite [] sprites = Resources.LoadAll<Sprite> ("Items/ItemsVer1");
	private static float defDur = 1;
	public static Dictionary<string, Item> ItemDict = new Dictionary<string, Item> {
				{ "Axe", new Tool ("Axe", sprites[2], "Good for killing trees (and anything really) \nbut quite heavy", 5, defDur, DamageType.Woodcutting, 10, 10, 0.5f)},
				{ "Branch", new Item ("Branch", sprites[9], "Tree arm", 2)},
				{ "Log", new Item ("Log", sprites[10], "Tree body", 6)},
				{ "Pickaxe", new Tool ("Pickaxe", sprites[1], "Good for breaking rocks", 3, defDur, DamageType.Rockbreaking, 10, 10, 0.5f)},
				{ "Stone", new Item ("Stone", sprites[8], "It's a stone.", 3)},
				{ "Stone axe", new Tool ("Stone axe", sprites[5], "A primitive axe", 3, defDur, DamageType.Woodcutting, 10, 10, 0.5f)},
				{ "Sword", new Tool ("Sword", sprites[0], "Useful for murder", 2, defDur, DamageType.Slicing, 10, 10, 0.5f)},

	};

}
