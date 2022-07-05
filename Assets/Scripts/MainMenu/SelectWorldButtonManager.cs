using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Text.RegularExpressions;


public class SelectWorldButtonManager : MonoBehaviour
{
    public GameObject button;

    // Start is called before the first frame update
    void Start()
    {
        string [] pathNames = Directory.GetFiles(Application.persistentDataPath);
        
        foreach (string pathName in pathNames) {
            string fileName = NonOverlap(Application.persistentDataPath, pathName);
            fileName = fileName.Remove(0, 1);

            if (fileName[0] != '.') {
                GameObject obj = Instantiate(button, this.transform);
                obj.GetComponentsInChildren<TextMeshProUGUI>()[0].text = WorldName.ToDisplayName(fileName);
            }
        }
    }

    public static string NonOverlap(string s1, string s2)
    {
        var re = string.Join("?", s1.ToCharArray()) + "?";
        var m = Regex.Match(s2, re);
        int s = m.Index + m.Length;
        return s2.Substring(s);
    }
}
