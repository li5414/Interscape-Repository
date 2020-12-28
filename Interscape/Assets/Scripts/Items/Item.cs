using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
	public string itemName;
	public Sprite icon;
	public string description;
	public float weight;
	
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

