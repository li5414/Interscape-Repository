using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour {
    public GameObject inventory;
    public GameObject itemTooltip;
    public GameObject hotbar;
    public GameObject cursor;

    public GameObject[] escMenuObjects;

    private bool hideHotbar;

    private bool isEscMenuActive = false;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F1) && inventory.activeSelf == false && !isEscMenuActive) {
            hideHotbar = !hideHotbar;
            hotbar.SetActive(!hideHotbar);
        }

        if (Input.GetKeyDown(KeyCode.E) && !isEscMenuActive) {
            bool state = inventory.activeSelf;
            if (state)
                itemTooltip.SetActive(false);
            inventory.SetActive(!state);

            if (!hideHotbar) {
                hotbar.SetActive(state);
            }
            cursor.SetActive(state);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            inventory.SetActive(false);
            hotbar.SetActive(false);
            itemTooltip.SetActive(false);
            isEscMenuActive = !escMenuObjects[0].activeSelf;
            foreach (GameObject obj in escMenuObjects) {
                obj.SetActive(!obj.activeSelf);
            }
        }
    }
}
