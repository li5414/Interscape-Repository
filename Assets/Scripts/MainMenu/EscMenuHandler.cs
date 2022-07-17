using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscMenuHandler : MonoBehaviour {
    public ChangeScene changeScene;

    public void handleSaveButton() {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<DataPersistenceManager>().SaveGame();
    }

    public void handleSaveAndLeaveButton() {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<DataPersistenceManager>().SaveGame();
        changeScene.GotoScene();
    }
}
