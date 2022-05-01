using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldingSlot : MonoBehaviour {

    private Item item;
    private int quantity;
    private ItemSlot holdingFrom;
    private Image image;

    void Start() {
        image = gameObject.GetComponent<Image>();
        image.enabled = false;
    }

    public bool HoldItem(Item item, ItemSlot holdingFrom, int quantity) {
        if (this.item != null) {
            Debug.LogError("Refused to hold something because already holding something");
            return false;
        }
        if (item == null || quantity <= 0) {
            Debug.LogError("Tried to hold nothing");
            return false;
        }
        if (holdingFrom == null) {
            Debug.LogError("The itemSlot provided is null");
            return false;
        }

        this.item = item;
        this.holdingFrom = holdingFrom;
        this.quantity = quantity;
        image.enabled = true;
        image.sprite = item.icon;
        if (item.iconColour != null)
            image.color = item.iconColour.Value;
        else {
            image.color = Color.white;
        }
        return true;
    }

    public Item RemoveHoldItem() {
        Item returnItem = item;
        item = null;
        image.enabled = false;
        quantity = 0;
        return returnItem;
    }

    public int GetQuantity() {
        return quantity;
    }
    public Item GetItem() {
        return item;
    }

    public ItemSlot GetHoldingFrom() {
        return holdingFrom;
    }

    public bool IsHolding() {
        return item != null;
    }
}
