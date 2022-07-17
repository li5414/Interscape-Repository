using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToggler : MonoBehaviour
{
    public GameObject [] mainMenuUI;
    public GameObject [] selectWorldUI;
    public GameObject [] newWorldUI;

    private Dictionary<string, GameObject[]> pageDictionary = new Dictionary<string, GameObject[]>();

    void Start() {
        pageDictionary.Add("mainMenuUI", mainMenuUI);
        pageDictionary.Add("selectWorldUI", selectWorldUI);
        pageDictionary.Add("newWorldUI", newWorldUI);
    }

    public void toggleOn(string page) {
        foreach (GameObject obj in pageDictionary[page]) {
            obj.SetActive(true);
        }
    }

    public void toggleOff(string page) {
        foreach (GameObject obj in pageDictionary[page]) {
            obj.SetActive(false);
        }
    }
}
