using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	
	[SerializeField] Image image;
	[SerializeField] ItemTooltip tooltip;

	private Item _item;
	public Item Item {
		get { return _item; }
		set {
			_item = value;

			// disable image component if no item in slot
			if (_item == null) {
				image.enabled = false;
			} else {
				image.sprite = _item.Icon;
				image.enabled = true;
			}
		}
	}

	protected virtual void OnValidate()
	{
		if (image == null)
			image = GetComponent<Image> ();
		if (tooltip == null) {
			tooltip = FindObjectOfType<ItemTooltip> ();
			
		}
			

	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log (tooltip == null);
		tooltip.ShowTooltip(Item);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		tooltip.HideTooltip();
	}
}
