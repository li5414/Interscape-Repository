using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
	private Inventory inventory;
	private bool isInteractible;
	public Color defaultColor;
	public Color interactableColor;
	private SpriteRenderer image;
	public float minDistance = 1;
	public float coolDown = 0;
	private Transform parent;

    // Start is called before the first frame update
    void Start()
    {
		inventory = GetComponentInParent<Inventory> ();
		image = GetComponent<SpriteRenderer> ();
		parent = GameObject.FindGameObjectWithTag ("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
		coolDown -= Time.deltaTime;
        
    }

	private void OnCollisionStay2D (Collision2D collision)
	{
		if (Mathf.Abs (transform.position.x - parent.position.x) < minDistance &&
			Mathf.Abs (transform.position.y - parent.position.y) < minDistance && coolDown <= 0) {
			isInteractible = true;
		} else {
			isInteractible = false;
			return;
		}

		//if (Input.GetMouseButtonDown(0)) {
		Harvestable obj = collision.gameObject.GetComponent<Harvestable> ();
		Item item = inventory.getSelectedItem ();
		if (obj && item) {
			if (item is Tool && ((Tool)item).getDamageType () == obj.getDamageType ()) {
				obj.harvest ((Tool)item);
				((Tool)item).decreaseDurability (inventory);
				Debug.Log ("Decreased durability :)");
			}
				
		}
		
	}
}
