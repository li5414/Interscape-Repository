using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu]
public class Item //: ScriptableObject
{
	public string itemName;
	public Sprite icon;
	public string description; // could optimise further by making an itemtemplate class
	public float weight;


	public Item(string itemName)
	{
		Item item;
		ItemRes.ItemDict.TryGetValue (itemName, out item);

		if (item != null) {
			this.itemName = item.itemName;
			this.icon = item.icon;
			this.description = item.description;
			this.weight = item.weight;
		} else {
			Debug.Log ("Item was not found");
		}
	}

	public Item (string itemName, Sprite icon, string description, float weight)
	{
		this.itemName = itemName;
		this.icon = icon;
		this.description = description;
		this.weight = weight;
	}

}

public enum Quality {
	BarelyRecognisable,
	Poor,
	Feeble,
	OkayIGuess,
	Decent,
	QuiteGood,
	Excellent,
	Flawless
}

// the durability of an item is tied to its quality
// master quality items last longer but as they get used their quality decreases until they break
// depending on skill you can only craft up to certain qualities
// when an item breaks maybe you get some materials back?

