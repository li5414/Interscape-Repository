using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Tool : Item
{
	[SerializeField] float durability;
	[SerializeField] Quality quality;
	[SerializeField] float damage;
	[SerializeField] DamageType damageType;
	[SerializeField] float baseDurability = 10;
	[SerializeField] float baseDamage = 10;
	public float coolDown = 0.5f;


	public float getDamage()
	{
		return damage;
	}
	public float getDurability ()
	{
		return durability;
	}
	public Quality getQuality ()
	{
		return quality;
	}
	public DamageType getDamageType ()
	{
		return damageType;
	}

	public void OnValidate ()
	{
		refreshQualities ();
	}

	public void refreshQualities()
	{
		
		if (durability < baseDurability * 0.25) {
			quality = Quality.BarelyRecognisable;
			damage = baseDamage * 0.25f;
		} else if (durability < baseDurability * 0.5) {
			quality = Quality.Poor;
			damage = baseDamage * 0.5f;
		} else if (durability < baseDurability * 0.75) {
			quality = Quality.Feeble;
			damage = baseDamage * 0.75f;
		} else if (durability < baseDurability) {
			quality = Quality.OkayIGuess;
			damage = baseDamage;
		} else if (durability < baseDurability * 1.5) {
			quality = Quality.Decent;
			damage = baseDamage * 1.5f;
		} else if (durability < baseDurability * 2) {
			quality = Quality.QuiteGood;
			damage = baseDamage * 2;
		} else if (durability < baseDurability * 3) {
			quality = Quality.Excellent;
			damage = baseDamage * 3;
		} else {
			quality = Quality.Flawless;
			damage = baseDamage * 4;
		}
	}

	public void decreaseDurability (Inventory inventory)
	{
		durability--;
		refreshQualities ();

		if (durability < 0) {
			inventory.RemoveSelectedItem ();
		}
	}
}

public enum DamageType {
	Woodcutting,
	Rockbreaking,
	Slicing,
	Digging

}

//highquality = more damage? but durability stays the same?
