using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
	private Inventory inventory;
	private bool isInteractable;
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

    // Update is called once per frame
    void Update()
    {
		if (coolDown > 0)
			coolDown -= Time.deltaTime;

		// right click
        if (Input.GetMouseButton (1) && coolDown <= 0) {
			coolDown = 0.3f;
			Item item = inventory.getSelectedItem ();
			if (item != null && item is BuildableItem) {
				// TODO green hightlight on cursor
				((BuildableItem)item).BuildItemAt(transform.position, inventory);
			}
		}
    }

	void OnTriggerStay2D (Collider2D other)
	{
		
		image.color = defaultColor;

		Harvestable obj = other.gameObject.GetComponent<Harvestable> ();
		Item item = inventory.getSelectedItem ();
		if (obj && item != null) {
			if (item is Tool && ((Tool)item).getDamageType () == obj.getDamageType ()) {
				image.color = interactableColor;

				if (Input.GetMouseButton (0) && coolDown <= 0) {
					obj.harvest ((Tool)item);
					((Tool)item).decreaseDurability (inventory);
					coolDown = ((Tool)item).coolDown;
					//Debug.Log ("Decreased durability :)");
				}
			}
				
		}

	}

	void OnTriggerExit2D (Collider2D other)
	{
		//Debug.Log ("Exit");
		image.color = defaultColor;
		isInteractable = false;
	}
}
