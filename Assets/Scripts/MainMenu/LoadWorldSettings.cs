using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadWorldSettings : MonoBehaviour
{
    private static string fileName;

    public static bool SetFileName(string displayName) {
        if (NewWorldSettings.CanSetFileName(displayName)) {
            fileName = NewWorldSettings.ToFileName(displayName);
            return true;
        }
        return false;
    }

    public static string GetFileName() {
        return fileName;
    }
}
