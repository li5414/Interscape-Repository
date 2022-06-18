using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour {
    public ItemSlot[] itemSlots;
    private ItemSlot selected;
    private SpriteRenderer playerHoldItem;
    public static Color selectedColor = new Color(1, 1, 1, 0.2f);
    private int prevSelected;
    private int currentSelected;
    private float minThreshold = 0.1f;
    public GameObject toolCursor;
    public static int HOTBAR_LENGTH = 10;

    private Inventory inventory;

    public void Initialise() {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();

        itemSlots = new ItemSlot[HOTBAR_LENGTH];
        for (int i = 0; i < itemSlots.Length; i++) {
            itemSlots[i] = GetComponentsInChildren<ItemSlot>()[i];
        }

        selected = itemSlots[0];
        prevSelected = 0;
        currentSelected = 0;
        playerHoldItem = GameObject.FindGameObjectWithTag("PlayerHoldItem").GetComponent<SpriteRenderer>();
        updateSelected(0, 0);
    }

    public int GetSelectedSlotNumber() {
        return currentSelected;
    }
    public Item GetSelectedItem() {
        return selected.GetItem();
    }
    public void SyncWithInventory() {
        for (int i = 0; i < HOTBAR_LENGTH; i++) {
            itemSlots[i].SetItems(inventory.itemSlots[i].GetItem(), inventory.itemSlots[i].GetQuantity());
        }
        updateSelectedUI();
    }

    private void Update() {
        if (Input.mouseScrollDelta.y > minThreshold) {
            updateSelected(currentSelected, currentSelected + 1);
        } else if (Input.mouseScrollDelta.y < 0 - minThreshold) {
            updateSelected(currentSelected, currentSelected - 1);
        } else if (Input.inputString != "") {
            int number;
            bool is_a_number = Int32.TryParse(Input.inputString, out number);
            if (is_a_number && number > 0 && number < 10) {
                updateSelected(currentSelected, number - 1);
            } else if (is_a_number && number == 0) {
                updateSelected(currentSelected, 9);
            }
        }
    }

    private void updateSelected(int from, int to) {
        prevSelected = from;
        currentSelected = to;

        if (currentSelected > 9)
            currentSelected = 0;
        else if (currentSelected < 0)
            currentSelected = 9;

        updateSelectedUI();
        ToggleCursor();
    }

    private void updateSelectedUI() {
        // update item sprites
        selected = itemSlots[currentSelected];
        if (selected.HasItem()) {
            playerHoldItem.sprite = selected.GetItem().icon;
            if (selected.GetItem().iconColour != null)
                playerHoldItem.color = selected.GetItem().iconColour.Value;
            else
                playerHoldItem.color = Color.white;
            playerHoldItem.enabled = true;
        } else {
            playerHoldItem.enabled = false;
        }

        //change colors of item slots
        itemSlots[prevSelected].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        selected.GetComponent<Image>().color = selectedColor;

    }

    public void ToggleCursor() {
        if (selected.GetItem() is Tool || selected.GetItem() is BuildableItem) {
            toolCursor.SetActive(true);
        } else {
            toolCursor.SetActive(false);
        }
    }

}
