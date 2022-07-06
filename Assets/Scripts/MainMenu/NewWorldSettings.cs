using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Security.Cryptography;
using System.Text;
using System;

public class NewWorldSettings : MonoBehaviour
{
    private static string fileName;
    private static int seed;

    public TMP_InputField fileNameTextInput;
    public TMP_InputField seedTextInput;
    public ChangeScene changeScene;

    public void ApplySettingsAndChangeScene() {
        if (SetFileName(fileNameTextInput.text)) {
            SetSeed(seedTextInput.text);
            changeScene.GotoScene();
        }
        else
            Debug.LogError("An error occured while creating a world with this name");
    }

    public static bool SetFileName(string displayName) {
        if (CanSetFileName(displayName)) {
            fileName = ToFileName(displayName);
            return true;
        }
        return false;
    }

    public static string GetFileName() {
        return fileName;
    }

    public static void SetSeed(string seedString) {
        seed = hashString(seedString);
    }

    public static int GetSeed() {
        return seed;
    }

    public static string ToFileName(string name) {
        foreach (char c in System.IO.Path.GetInvalidFileNameChars()) {
            name = name.Replace(c, '_');
        }
        name = name.Replace(' ', '_');
        return name;
    }

    public static bool CanSetFileName(string displayName) {
        return (displayName != null && displayName.Length > 0 && displayName.Length < 100 && displayName[0] != '.');
    }

    public static string ToDisplayName(string name) {
        return name.Replace('_', ' ');
    }

    private static int hashString(string str) {
        MD5 md5Hasher = MD5.Create();
        var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
        return BitConverter.ToInt32(hashed, 0);
    }
}
