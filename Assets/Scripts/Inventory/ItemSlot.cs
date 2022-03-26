using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
	
	[SerializeField] Image image;
	[SerializeField] ItemTooltip tooltip;
	[SerializeField] int number;
	public bool isHotbar;

	private Inventory inventory;
	private GraphicRaycaster graphicRaycaster;
	private Canvas canvas;


	void Awake()
	{
		inventory = GameObject.FindGameObjectWithTag ("Inventory").GetComponent<Inventory>();
		image = GetComponentsInChildren<Image> ()[1];


		if (inventory == null) {
			Debug.Log ("inventoyr null");
		}

		if (!canvas) {
			canvas = GameObject.FindGameObjectWithTag ("Canvas").GetComponent<Canvas> ();
			graphicRaycaster = canvas.GetComponent<GraphicRaycaster> ();
		}

		string numbersOnly = Regex.Replace (gameObject.name, "[^0-9]", "");
		number = int.Parse (numbersOnly);

		tooltip = GameObject.FindGameObjectWithTag ("ItemTooltip").GetComponent<ItemTooltip> ();
		if (tooltip == null) {
			Debug.Log ("Tooltip null????");
		}
	}

	private Item _item;
	public Item Item {
		get { return _item; }
		set {
			_item = value;

			// disable image component if no item in slot
			if (_item == null) {
				image.enabled = false;
			} else {
				image.sprite = _item.icon;
				if (_item.iconColour != null) {
					image.color = _item.iconColour.Value;
				}
				image.enabled = true;
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!isHotbar)
			tooltip.ShowTooltip(Item);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		if (!isHotbar)
			tooltip.HideTooltip();
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		if (!isHotbar)
			inventory.SwapToHolding (number);
		//Debug.Log ("Pointer down at " + number);
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		if (!isHotbar) {
			var results = new List<RaycastResult> ();
			graphicRaycaster.Raycast (eventData, results);
			// Check all hits.
			//Debug.Log (results.Count);

			foreach (var hit in results) {
				// If we found slot.
				var slot = hit.gameObject.GetComponent<ItemSlot> ();
				if (slot) {
					inventory.SwapToInventory (slot.number);
					//Debug.Log ("Pointer up at " + slot.number);

					return;
				}
				//Debug.Log (hit.gameObject.name);
			}

			foreach (var hit in results) {
				// If we found inventory
				var inv = hit.gameObject.name.Equals ("Inv Rect");
				if (inv) {
					inventory.CancelHold ();
					return;
				}
			}

			// else we must have hit nothing
			inventory.DropHoldItem ();
		}

	}
	public void OnDrag (PointerEventData eventData)
	{
		//throw new NotImplementedException ();
	}
}
