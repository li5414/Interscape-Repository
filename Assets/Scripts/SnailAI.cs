using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailAI : MonoBehaviour
{
	public float speed = 1f;
	int range = 5;
	Vector2 wayPoint;
	public GameObject player;
	private float restingTime;
	private bool isRunning;
	[Space]

	private Animator anim;
	public AnimationClip idle;
	public AnimationClip walk;
	public AnimationClip run;
	public AnimationClip standup;
	public AnimationClip sitdown;
	

	// Start is called before the first frame update
	void Start()
    {
		anim = GetComponent<Animator> ();
		Wander ();
	}

    // Update is called once per frame
    void Update()
    {
		if (restingTime > 0) {
			restingTime -= Time.deltaTime;
		}


		if ((new Vector2 (transform.position.x, transform.position.y) -
					new Vector2 (player.transform.position.x, player.transform.position.y)).magnitude < 8) {
			restingTime = 0;
			RunAway();
		}

		if (restingTime <= 0) {

			// reached destination
			if (Vector2.Distance (transform.position, wayPoint) < 0.001) {
				restingTime = Random.Range (0, 7);
				if (isRunning && restingTime > 0) {
					isRunning = false;
					anim.Play ("snail_sitdown 0");
				} else if (isRunning) {
					isRunning = false;
					
					anim.Play (sitdown.name);
				}
				else {
					anim.Play (idle.name);
				}
				
				Wander ();
			}
			// switch animations
			else if (!isRunning) {
				if (anim.GetCurrentAnimatorStateInfo (0).IsName (run.name))
					anim.Play (sitdown.name);
				else if (anim.GetCurrentAnimatorStateInfo (0).IsName (idle.name))
					anim.Play (walk.name);
			}

			// actually move
			float step = speed * Time.deltaTime;
			Vector2 tempVector2 = Vector2.MoveTowards (transform.position, wayPoint, step);
			transform.position = new Vector3 (tempVector2.x, tempVector2.y, transform.position.z);
		}

		}

	public void RunAway() {
		//Debug.Log ("I'm running away!");
		if (!(anim.GetCurrentAnimatorStateInfo (0).IsName (run.name) || anim.GetCurrentAnimatorStateInfo (0).IsName (standup.name))) {
			anim.Play (standup.name);
		}

		isRunning = true;
		speed = 4f;
		Vector2 direction = new Vector2 (transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y);
		direction = direction.normalized * 16;
		
		wayPoint = (new Vector2 (transform.position.x + direction.x, transform.position.y + direction.y));
	}

	public void Wander()
	{
		// does nothing except pick a new destination to go to
		//Debug.Log ("I'm wandering");
		

		speed = 1f;
		wayPoint = new Vector2 (Random.Range (transform.position.x - range, transform.position.x + range),
			Random.Range (transform.position.y - range, transform.position.y + range));
	}

	public void SearchForFood() {

	}

	public void Sleep()
	{

	}

}