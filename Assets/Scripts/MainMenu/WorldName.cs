using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldName : MonoBehaviour
{
    private static string currentWorldFileName;
    public TMP_InputField textInput;
    public ChangeScene changeScene;

    public void SetCurrentWorldFileNameFromTextInput() {
        if (SetCurrentWorldFileName(textInput.text))
            changeScene.GotoScene();
        else
            Debug.LogError("An error occured while creating a world with this name");
    }

    public static bool SetCurrentWorldFileName(string displayName) {
        if (displayName != null && displayName.Length > 0 && displayName.Length < 100 && displayName[0] != '.') {
            currentWorldFileName = ToFileName(displayName);
            return true;
        }
        return false;
    }

    public static string GetCurrentWorldFileName() {
        return currentWorldFileName;
    }

    public static string ToFileName(string name) {
        foreach (char c in System.IO.Path.GetInvalidFileNameChars()) {
            name = name.Replace(c, '_');
        }
        name = name.Replace(' ', '_');
        return name;
    }

    public static string ToDisplayName(string name) {
        return name.Replace('_', ' ');
    }
}
