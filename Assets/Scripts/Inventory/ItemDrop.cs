using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrop : MonoBehaviour {
    [SerializeField] Item item;
    private SpriteRenderer image;
    private int quantity;
    private ItemDropSettings itemDropSettings;

    void Awake() {
        itemDropSettings = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ItemDropSettings>();
        image = GetComponentsInChildren<SpriteRenderer>()[1];
    }
    private static float dropRadius = 0.8f;

    public bool pickupItem(ItemDrop itemDrop) {
        if (itemDropSettings.inventory.AddItems(itemDrop.item, itemDrop.quantity)) {
            Destroy(itemDrop.gameObject);
            return true;
        }
        return false;
    }

    public void OnMouseOver() {
        if (Input.GetMouseButton(1)) {
            pickupItem(this);
        }
    }

    public static bool DropItemsAt(Item item, int quantity, Vector3 at) {
        if (item == null)
            return false;

        GameObject obj = Instantiate(ItemDropSettings.itemDropPrefab, ItemDropSettings.parent.transform);
        ItemDrop drop = obj.GetComponent<ItemDrop>();
        drop.item = item;
        drop.image.sprite = item.icon;
        if (item.iconColour != null)
            drop.image.color = item.iconColour.Value;
        drop.quantity = quantity;

        Vector2 pos = Random.insideUnitCircle * dropRadius;
        obj.transform.position = new Vector3(at.x + pos.x, at.y + pos.y, at.z);
        return true;
    }

    public static bool DropItems(Item item, int quantity) {
        ItemDropSettings itemDropSettings = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ItemDropSettings>();
        return DropItemsAt(item, quantity, itemDropSettings.player.transform.position);
    }
}
