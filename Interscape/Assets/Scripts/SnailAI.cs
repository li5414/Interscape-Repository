using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailAI : MonoBehaviour
{
	float speed = 1f;
	int range = 5;
	Vector2 wayPoint;
	public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
		StartCoroutine (Decide ());
	}

    // Update is called once per frame
    void Update()
    {
		float step = speed * Time.deltaTime;

		// actually move
		Vector2 tempVector2 = Vector2.MoveTowards (transform.position, wayPoint, step);
		transform.position = new Vector3 (tempVector2.x, tempVector2.y, transform.position.z);
		if ((new Vector2 (transform.position.x, transform.position.y) -
				new Vector2 (player.transform.position.x, player.transform.position.y)).magnitude < 3) {
			StopCoroutine (Decide ());
			StartCoroutine (Decide ());
		}
	}

	public void RunAway() {
		Debug.Log ("I'm running away!");
		speed = 2f;
		wayPoint = (new Vector2 (transform.position.x, transform.position.y) -
				new Vector2 (player.transform.position.x, player.transform.position.y));
	}

	public void Wander()
	{
		// does nothing except pick a new destination to go to
		Debug.Log ("I'm wandering");
		speed = 1f;
		wayPoint = new Vector2 (Random.Range (transform.position.x - range, transform.position.x + range),
			Random.Range (transform.position.y - range, transform.position.y + range));
	}

	public void SearchForFood() {

	}

	public void Sleep()
	{

	}

	IEnumerator Decide ()
	{
		while(true) {
			if ((new Vector2 (transform.position.x, transform.position.y) -
				new Vector2 (player.transform.position.x, player.transform.position.y)).magnitude < 3) {
				RunAway ();
			} else {
				Wander ();
				
			}
			yield return new WaitForSeconds (Random.Range (1, 10));

		}
	}

}