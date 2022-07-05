using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectWorldButtonHandler : MonoBehaviour
{
    public string fileName;
    public ChangeScene changeScene;

    public void LoadWorld() {
        if (WorldName.SetCurrentWorldFileName(fileName))
            changeScene.GotoScene();
        else
            Debug.LogError("An error occured while loading world: " + fileName);
    }
}
