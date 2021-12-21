using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestroyableObj : MonoBehaviour
{
	private Animator anim;
	public AnimationClip damageClip;
	[SerializeField] float health = 100;
	[SerializeField] DamageType damageType;
	public string dropItemName;
	public AnimationClip deathClip;
	public GameObject parentObj;

	public bool isUsingRuleTiles;
	private Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
		anim = GetComponent<Animator> ();
		if (parentObj == null) {
			parentObj = this.gameObject;
		}
		if (isUsingRuleTiles) {
			tilemap = GameObject.FindWithTag("RuleTilemap").GetComponent<Tilemap>();
		}
    }

	public void damage(Tool tool)
	{
		health -= tool.getDamage ();
		
		// check health reaches zero
		if (health < 0) {
			if (deathClip != null) {
				StartCoroutine (Die (deathClip.length));
				anim.Play (deathClip.name);
			} else {
				actuallyDie ();
			}
			return;
		}

		// else play damage animation
		if (damageClip != null)
			anim.Play (damageClip.name);
	}

	public DamageType getDamageType()
	{
		return damageType;
	}

	IEnumerator Die (float delay)
	{
		//play your sound
		yield return new WaitForSeconds (delay);
		actuallyDie ();
	}

	private void actuallyDie()
	{
		if (dropItemName != "") {
			ItemDrop.dropItemAt (ItemRes.MakeItemCopy(dropItemName), this.transform.position, 1);
		}
		if (tilemap != null) {
			Vector3Int cellPos = tilemap.WorldToCell(this.transform.position);
			tilemap.SetTile(cellPos, null);
		}
		Destroy (parentObj);
	}
}


