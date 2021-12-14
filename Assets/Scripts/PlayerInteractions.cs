using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
	private Inventory inventory;
	private Color defaultColor;
	public Color interactableColor;
	private SpriteRenderer image;
	public float minDistance = 1;
	public float coolDown = 0;
	private Transform parent;

    // Start is called before the first frame update
    void Start()
    {
		inventory = GameObject.FindGameObjectWithTag ("Inventory").GetComponent<Inventory>();
		image = GetComponent<SpriteRenderer> ();
		defaultColor = image.color;
		parent = GameObject.FindGameObjectWithTag ("Player").transform;
    }

    void FixedUpdate()
    {
		if (coolDown > 0) {
			coolDown -= Time.deltaTime;
			return;
		}

		// check for null selected item
		Item selectedItem = inventory.getSelectedItem ();
		if (selectedItem == null) {
			image.color = defaultColor;
			return;
		}

		// cast ray to get objects
		RaycastHit2D [] hits = Physics2D.RaycastAll(transform.position, -Vector2.zero);

		// ignore item drop objects
		RaycastHit2D hit = new RaycastHit2D();
		for (int i = 0; i < hits.Length; i++) {
			if (hits[i].transform != null && hits[i].transform.gameObject.tag != "ItemDrop") {
				hit = hits[i];
				break;
			}
		}

		// convert hit to gameobject
		GameObject hoveringOver = null;
		if (hit != null && hit.transform != null)
			hoveringOver = hit.transform.gameObject;

		// update color of cursor
		updateColour(hoveringOver);

		// if left click happens
		if (Input.GetMouseButton (0)) {
			if (selectedItem is Tool && hoveringOver != null) {
				Harvestable hoveringOverComponent = hoveringOver.GetComponent<Harvestable> ();
				if (hoveringOverComponent != null) {

					// if damage types match
					if (hoveringOverComponent.getDamageType() == ((Tool)selectedItem).getDamageType ()) {
						hoveringOverComponent.harvest ((Tool)selectedItem);
						((Tool)selectedItem).decreaseDurability (inventory);
						coolDown = ((Tool)selectedItem).coolDown;
					}
				}
			}
			return;
		}

		// if right click happens
        if (Input.GetMouseButton (1)) {
			coolDown = 0.3f;
			if (selectedItem is BuildableItem && hoveringOver == null) {
				((BuildableItem)selectedItem).BuildItemAt(transform.position, inventory);
			}
		}
    }

	void updateColour(GameObject hoveringOver) {
		Item selectedItem = inventory.getSelectedItem ();
		image.color = defaultColor;

		if (selectedItem is BuildableItem && hoveringOver == null) {
			image.color = interactableColor;
			return;
		}
		if (selectedItem is Tool && hoveringOver != null) {
			Harvestable hoveringOverComponent = hoveringOver.GetComponent<Harvestable> ();
			if (hoveringOverComponent != null) {
				if (hoveringOverComponent.getDamageType() == ((Tool)selectedItem).getDamageType ())
					image.color = interactableColor;
			}
		}
	}

	// void OnTriggerStay2D (Collider2D other)
	// {
		
	// 	image.color = defaultColor;

	// 	Harvestable obj = other.gameObject.GetComponent<Harvestable> ();
	// 	Item item = inventory.getSelectedItem ();
	// 	if (obj && item != null) {
	// 		if (item is Tool && ((Tool)item).getDamageType () == obj.getDamageType ()) {
	// 			image.color = interactableColor;

	// 			if (Input.GetMouseButton (0) && coolDown <= 0) {
	// 				obj.harvest ((Tool)item);
	// 				((Tool)item).decreaseDurability (inventory);
	// 				coolDown = ((Tool)item).coolDown;
	// 			}
	// 		}
	// 	}
	// }

	// void OnTriggerExit2D (Collider2D other)
	// {
	// 	image.color = defaultColor;
	// }
}
