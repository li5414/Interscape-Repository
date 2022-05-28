using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BuildableItem : Item {
    public RuleTile buildingReference;
    public Tilemap tilemap;

    public BuildableItem(string itemName) : base(itemName) {
        Item item;
        ItemRes.ItemDict.TryGetValue(itemName, out item);

        if (item != null && item is BuildableItem) {
            this.buildingReference = ((BuildableItem)item).buildingReference;
            this.tilemap = ((BuildableItem)item).tilemap;
        }
    }
    public BuildableItem(string itemName, Sprite icon, string description, float weight, RuleTile buildingReference, string tilemap)
    : base(itemName, icon, description, weight) {
        this.buildingReference = buildingReference;
        this.tilemap = getTilemap(tilemap);
        this.iconColour = this.tilemap.gameObject.GetComponent<TilemapRenderer>().material.GetColor("_Color");
    }

    private Tilemap getTilemap(string tilemap) {
        return GameObject.FindWithTag("SystemPlaceholder").GetComponent<BuildingResources>().TilemapDict[tilemap];
    }

    public void BuildItemAt(Vector3 worldPos, Inventory inventory) {
        if (buildingReference != null) {
            tilemap.SetTile(tilemap.WorldToCell(worldPos), buildingReference);
            inventory.RemoveOneSelectedItem();
        } else {
            Debug.LogError("The building object for this item does not exist", buildingReference);
        }
    }
}
