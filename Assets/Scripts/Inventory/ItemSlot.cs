using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public bool isHotbar;
    private Image image;
    private ItemTooltip tooltip;
    // private int number;
    private Inventory inventory;
    private GraphicRaycaster graphicRaycaster;
    private Canvas canvas;
    private TextMeshProUGUI quantityText;

    private int quantity;
    private Item item;

    void Awake() {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        image = GetComponentsInChildren<Image>()[1];
        image.enabled = false;
        quantityText = GetComponentInChildren<TextMeshProUGUI>();
        updateQuantityText();

        if (inventory == null) {
            Debug.LogError("Inventory is null. Remember to set the 'Inventory' tag");
        }

        if (!canvas) {
            canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
            graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        }

        tooltip = GameObject.FindGameObjectWithTag("ItemTooltip").GetComponent<ItemTooltip>();
        if (tooltip == null) {
            Debug.LogError("Tooltip is null. Remember to set the 'ItemTooltop' tag");
        }

    }
    public bool HasItem() {
        return item != null && quantity > 0;
    }
    public Item GetItem() {
        return item;
    }
    private void updateItem(Item newItem) {
        // disable image component if no item in slot
        item = newItem;
        if (item == null) {
            image.enabled = false;
        } else {
            image.sprite = item.icon;
            if (item.iconColour != null) {
                image.color = item.iconColour.Value;
            }
            image.enabled = true;
        }
        updateQuantityText();
    }

    private void updateQuantityText() {
        quantityText.text = quantity.ToString();
        if (quantity <= 1) {
            quantityText.enabled = false;
        } else
            quantityText.enabled = true;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!isHotbar)
            tooltip.ShowTooltip(item);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!isHotbar)
            tooltip.HideTooltip();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (!isHotbar)
            inventory.SwapToHolding(this);
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!isHotbar) {
            var results = new List<RaycastResult>();
            graphicRaycaster.Raycast(eventData, results);

            foreach (var hit in results) {
                // If we found slot.
                var slot = hit.gameObject.GetComponent<ItemSlot>();
                if (slot) {
                    inventory.SwapToInventory(slot);
                    return;
                }
            }

            foreach (var hit in results) {
                // If we found inventory
                var inv = hit.gameObject.name.Equals("Inv Rect");
                if (inv) {
                    inventory.CancelHold();
                    return;
                }
            }

            // else we must have hit nothing
            inventory.DropHoldItem();
        }

    }
    public void OnDrag(PointerEventData eventData) {
        //throw new NotImplementedException ();
    }

    public bool CanAddItem(Item newItem) {
        if (newItem == null) {
            Debug.Log("Warning: newItem is null");
            return false;
        }
        if (item != null) {
            if (!item.itemName.Equals(newItem.itemName)) {
                return false;
            }
        }
        return true;
    }
    public bool CanAddItems(Item newItem, int quantity) {
        if (!CanAddItem(newItem) || quantity <= 0)
            return false;
        return true;
    }

    public bool AddItems(Item newItem, int quantity) {
        if (!CanAddItems(newItem, quantity)) {
            Debug.LogError("Error adding " + newItem.itemName + " to slot");
            return false;
        }
        this.quantity += quantity;
        updateItem(newItem);
        return true;
    }

    public void SetItems(Item newItem, int quantity) {
        this.quantity = quantity;
        this.updateItem(newItem);
    }

    public bool RemoveItems(int amount) {
        if (amount <= 0) {
            Debug.LogError("Invalid amount value provided");
            return false;
        }
        if (this.item == null) {
            Debug.Log("Warning: the itemSlot item was already null when trying to remove items");
            return false;
        }
        if (quantity >= amount) {
            quantity -= amount;
            updateQuantityText();
            if (quantity <= 0) {
                updateItem(null);
            }
            return true;
        }
        Debug.LogError("Error decreasing " + quantity + " items by " + amount);
        return false;
    }
    public int GetQuantity() {
        return quantity;
    }
}
