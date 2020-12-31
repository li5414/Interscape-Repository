using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvestable : MonoBehaviour
{
	private Animator anim;
	public AnimationClip clip;
	[SerializeField] float health = 100;
	[SerializeField] DamageType damageType;
	public string dropItemName;

    // Start is called before the first frame update
    void Start()
    {
		anim = GetComponent<Animator> ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void harvest(Tool tool)
	{
		health -= tool.getDamage ();
		
		//Debug.Log ("Hit");
		if (health < 0) {
			if (dropItemName != "") {
				ItemDrop.dropItemAt (new Item (dropItemName), this.transform.position, 1);
			}
			Destroy (gameObject);
		}

		anim.Play (clip.name);
	}

	public DamageType getDamageType()
	{
		return damageType;
	}
}


