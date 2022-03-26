using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BuildableItem : Item
{
    public RuleTile buildingReference;
    public BuildLayer buildLayer;
    public Tilemap tilemap;
    public Material? material;

    public BuildableItem (string itemName) : base (itemName)
	{
        Item item;
		ItemRes.ItemDict.TryGetValue (itemName, out item);
        
		if (item != null && item is BuildableItem) {
			this.buildingReference = ((BuildableItem)item).buildingReference;
            this.material = ((BuildableItem)item).material;
            this.buildLayer = ((BuildableItem)item).buildLayer;
		}
	}
    public BuildableItem (string itemName, Sprite icon, string description, float weight, RuleTile buildingReference, 
    BuildLayer buildLayer, Color? iconColour = null, Material? material = null) 
    : base (itemName, icon, description, weight, iconColour)
	{
        this.buildingReference = buildingReference;
        this.material = material;
        this.buildLayer = buildLayer;
	}

    public void BuildItemAt(Vector3 worldPos, Inventory inventory) {
        // ensure tilemap exists first
        if (!tilemap)
            tilemap = getTilemap(this.itemName, this.buildLayer, this.material);
        
        if (buildingReference != null) {
            tilemap.SetTile(tilemap.WorldToCell(worldPos), buildingReference);
            inventory.RemoveSelectedItem();
        }
        else {
            Debug.LogError("The building object for this item does not exist", buildingReference);
        }
    }

    public static Tilemap getTilemap(string itemName, BuildLayer buildLayer, Material? material = null) {
        // TODO figure out what is going to happen with the ground layer
        if (buildLayer == BuildLayer.FLOOR_LAYER) {
            GameObject parent = GameObject.FindWithTag("FloorParent");
            Transform existingTilemapObject = (parent.transform.Find(itemName + " Tilemap"));

            if (existingTilemapObject) {
                Tilemap potentialTilemap = existingTilemapObject.gameObject.GetComponent<Tilemap>();

                if (potentialTilemap) {
                    return potentialTilemap;
                } else {
                    Debug.LogError("Found transform with correct name but it had no Tilemap component", existingTilemapObject);
                }
            }
            return CreateTilemap(itemName + " Tilemap", parent, material);
        }
        // else if (buildLayer == BuildLayer.BUILDING_LAYER) {
        //     return GameObject.FindWithTag("RuleTilemap").GetComponent<Tilemap>();
        // }
        return GameObject.FindWithTag("RuleTilemap").GetComponent<Tilemap>();
    }

    public static Tilemap CreateTilemap(string tilemapName, GameObject parent, Material? material)
    {
        var go = new GameObject(tilemapName);
        var tm = go.AddComponent<Tilemap>();
        var tr = go.AddComponent<TilemapRenderer>();

        if (material)
            tr.material = material;

        // tm.tileAnchor = new Vector3(AnchorX, AnchorY, 0);
        go.transform.SetParent(parent.transform);
        // tr.sortingLayerName = "Main";
        return tm;
    }
}

public enum BuildLayer {
    GROUND_LAYER,
    FLOOR_LAYER,
    BUILDING_LAYER
}
