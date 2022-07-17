using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
	public string scene;
	public float delayRemaining = 0;

	public void GotoScene ()
	{
		if (delayRemaining <= 0)
			SceneManager.LoadScene (scene, LoadSceneMode.Single);
	}

	private void Update () {
		delayRemaining -= Time.deltaTime;
	}
}
