using System.Collections.Generic;
using UnityEngine;

public class Item {
    public string itemName;
    public Sprite icon;
    public string description;
    public float weight;
    public Color? iconColour;

    public Item(string itemName) {
        Item item;
        ItemRes.ItemDict.TryGetValue(itemName, out item);

        if (item != null) {
            this.itemName = item.itemName;
            this.icon = item.icon;
            this.description = item.description;
            this.weight = item.weight;
            this.iconColour = item.iconColour;
        } else {
            Debug.LogError("Item " + itemName + "cannot be created because it doesn't exist");
        }
    }

    public Item(string itemName, Sprite icon, string description, float weight, Color? iconColour = null) {
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
        this.weight = weight;
        this.iconColour = iconColour;
    }
}