using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour {
    [SerializeField] GameObject itemsParent;
    [SerializeField] GameObject hotbarParent;
    public ItemSlot[] itemSlots = new ItemSlot[50];
    [SerializeField] TextMeshProUGUI weightText;
    public float weight;
    private int count = 0;
    [SerializeField] Hotbar hotbar;

    public HoldingSlot holding;

    private void Start() {
        int i;
        for (i = 0; i < Hotbar.HOTBAR_LENGTH; i++) {
            itemSlots[i] = hotbarParent.GetComponentsInChildren<ItemSlot>()[i];
        }
        for (int j = i; j < itemSlots.Length; j++) {
            itemSlots[j] = itemsParent.GetComponentsInChildren<ItemSlot>()[j - i];
        }

        hotbar.Initialise();
        RecalculateWeight();
        count = refreshCount();

        // add items for debugging and testing
        AddItem(new Tool("Axe", 300));
        AddItem(new Tool("Pickaxe", 500));
        AddItem(new Tool("Sword", 50));
        AddItem(new Tool("Stone axe", 75));
        AddItem(new TerrainTool("Hoe", 75));
        AddItems(new BuildableItem("Mid Wood Floor"), 5);

        // hotbar.UpdateSelectedUI();
        gameObject.SetActive(false);
    }

    public Item GetSelectedItem() {
        return hotbar.GetSelectedItem();
    }
    public bool IsFull() {
        return count >= itemSlots.Length;
    }
    private int refreshCount() {
        int c = 0;
        for (int i = 0; i < itemSlots.Length; i++) {
            if (itemSlots[i].HasItem()) {
                c++;
            }
        }
        return c;
    }

    public void RecalculateWeight() {
        float sum = 0;
        for (int i = 0; i < itemSlots.Length; i++) {
            if (itemSlots[i].HasItem()) {
                sum += itemSlots[i].GetItem().weight;
            }
        }
        weight = sum;
        weightText.text = "Weight: " + sum;
        // update count while we are here
        count = refreshCount();
    }

    public bool AddItemAt(Item item, ItemSlot itemSlot) {
        if (itemSlot.AddItems(item, 1)) {
            RecalculateWeight();
            hotbar.SyncWithInventory();
            return true;
        }
        return false;
    }
    public bool AddItemsAt(Item item, ItemSlot itemSlot, int quantity) {
        if (itemSlot.AddItems(item, quantity)) {
            RecalculateWeight();
            hotbar.SyncWithInventory();
            return true;
        }
        return false;
    }

    public bool AddItem(Item item) {
        // find next suitable stackable slots, if any
        for (int i = 0; i < itemSlots.Length; i++) {
            if (itemSlots[i].HasItem() && itemSlots[i].CanAddItem(item)) {
                if (AddItemsAt(item, itemSlots[i], 1))
                    return true;
            }
        }

        // find next empty slot, if any
        for (int i = 0; i < itemSlots.Length; i++) {
            if (!itemSlots[i].HasItem()) {
                if (AddItemsAt(item, itemSlots[i], 1))
                    return true;
            }
        }
        return false;
    }

    public bool AddItems(Item item, int quantity) {
        // find next suitable stackable slots, if any
        for (int i = 0; i < itemSlots.Length; i++) {
            if (itemSlots[i].HasItem() && itemSlots[i].CanAddItem(item)) {
                if (AddItemsAt(item, itemSlots[i], quantity))
                    return true;
            }
        }

        // find next empty slot, if any
        for (int i = 0; i < itemSlots.Length; i++) {
            if (!itemSlots[i].HasItem()) {
                if (AddItemsAt(item, itemSlots[i], quantity))
                    return true;
            }
        }
        return false;
    }

    public void RemoveOneSelectedItem() {
        ItemSlot selectedItemSlot = itemSlots[hotbar.GetSelectedSlotNumber()];
        RemoveItemsAt(selectedItemSlot, 1);
    }

    public bool RemoveItemsAt(ItemSlot itemSlot, int quantity) {
        if (itemSlot.RemoveItems(quantity)) {
            RecalculateWeight();
            hotbar.SyncWithInventory();
            return true;
        }
        return false;
    }

    public bool SwapToHolding(ItemSlot itemSlot) {
        if (holding.HoldItem(itemSlot.GetItem(), itemSlot, itemSlot.GetQuantity()))
            RemoveItemsAt(itemSlot, itemSlot.GetQuantity());
        else {
            Debug.LogError("There was an error holding the item");
            return false;
        }
        return true;
    }

    public bool CancelHold() {
        if (!holding.IsHolding()) {
            Debug.LogError("Tried to cancel holding nothing");
            return false;
        }
        SwapToInventory(holding.GetHoldingFrom());
        return true;
    }

    public bool SwapToInventory(ItemSlot itemSlot) {
        int quantity = holding.GetQuantity();
        Item item = holding.GetItem();
        if (item == null) {
            Debug.LogError("Holding item is null");
            return false;
        }

        // if adding the item normally doesn't succeed, return it back to where the other item was
        if (itemSlot.CanAddItems(item, quantity)) {
            AddItemsAt(item, itemSlot, quantity);
            holding.RemoveHoldItem();
        } else {
            CancelHold();
            swapItems(itemSlot, holding.GetHoldingFrom());
        }
        return true;
    }

    public bool DropHoldItem() {
        if (!holding.IsHolding()) {
            Debug.LogError("Tried to drop null item");
            return false;
        }
        ItemDrop.dropItem(holding.RemoveHoldItem());
        return true;
    }

    private void swapItems(ItemSlot itemSlot1, ItemSlot itemSlot2) {
        Debug.Log("Swapping items...");
        Item item1 = itemSlot1.GetItem();
        int quantity1 = itemSlot1.GetQuantity();

        Item item2 = itemSlot2.GetItem();
        int quantity2 = itemSlot2.GetQuantity();

        RemoveItemsAt(itemSlot1, quantity1);
        RemoveItemsAt(itemSlot2, quantity2);

        itemSlot2.SetItems(item1, quantity1);
        itemSlot1.SetItems(item2, quantity2);

        RecalculateWeight();
        hotbar.SyncWithInventory();
    }

}
