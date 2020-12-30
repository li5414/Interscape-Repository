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
        
    }

	void OnTriggerStay2D (Collider2D other)
	{
		//Debug.Log ("collision");
		if (//Mathf.Abs (transform.position.x - parent.position.x) < minDistance &&
			/*Mathf.Abs (transform.position.y - parent.position.y) < minDistance &&*/ coolDown <= 0) {
			isInteractable = true;
		} else {
			isInteractable = false;
			return;
		}
		image.color = defaultColor;

		//if (Input.GetMouseButtonDown(0)) {
		Harvestable obj = other.gameObject.GetComponent<Harvestable> ();
		Item item = inventory.getSelectedItem ();
		if (obj && item) {
			if (item is Tool && ((Tool)item).getDamageType () == obj.getDamageType ()) {
				image.color = interactableColor;

				if (Input.GetMouseButtonDown (0)) {
					obj.harvest ((Tool)item);
					((Tool)item).decreaseDurability (inventory);
					coolDown = ((Tool)item).coolDown;
					//Debug.Log ("Decreased durability :)");
				}
				return;
			}
				
		}

	}

	void OnTriggerExit2D (Collider2D other)
	{
		image.color = defaultColor;
		isInteractable = false;
	}
}
