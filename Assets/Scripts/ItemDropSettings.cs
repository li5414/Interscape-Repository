using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropSettings : MonoBehaviour {

    public static GameObject itemDropPrefab;
    public GameObject player;
    public Inventory inventory;
    public static GameObject parent;

    // Start is called before the first frame update
    void Start() {
        parent = GameObject.FindGameObjectWithTag("ItemDropParent");
        itemDropPrefab = Resources.Load<GameObject>("Items/ItemDrop");
    }

}
