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
				image.sprite = _item.icon;
				image.enabled = true;
			}
		}
	}

	protected virtual void OnValidate()
	{
		if (image == null) {
			image = GetComponent<Image> ();
			Debug.Log ("image null");
		}
		if (tooltip == null) {
			tooltip = FindObjectOfType<ItemTooltip> ();
			Debug.Log ("tooltip null");
		}
			

	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		tooltip.ShowTooltip(Item);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		tooltip.HideTooltip();
	}
}
