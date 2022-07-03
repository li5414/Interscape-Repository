using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmptyTextToggle : MonoBehaviour
{
    public GameObject parent;
    public GameObject objToToggle;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        int nChildren = parent.GetComponentsInChildren<Button>().GetLength(0);
        if (nChildren <= 0)
            objToToggle.SetActive(true);
        else
            objToToggle.SetActive(false);
    }
}
