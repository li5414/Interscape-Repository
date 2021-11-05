using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {
	
	/* initialise variables */
	public float speed;
	public bool isBurdened = true;
	bool right = true;
    Animator a;
    Transform player;
	private PlayerStats playerStats;
	public float burdenEffectCutoff = 0.75f;
	SpriteRenderer spriteR;
	private Facing facing;
	private bool isIdle;

	// hair objects
	public Sprite hairFront;
	public Sprite hairFrontside;
	public Sprite hairSide;
	public Sprite hairBackside;
	public Sprite hairBack;
	public GameObject hair;

	/* assign player position and animator */
	void Start() {
        a = GetComponent<Animator>();
        player = this.transform;
		playerStats = GetComponent<PlayerStats> ();
		spriteR = hair.GetComponent<SpriteRenderer> ();
		facing = Facing.BotRight;
	}

    void FixedUpdate() {

		// get input
		playerStats.isRunning = false;
		string input = getWASD ();
		setFacing (input);

		// idle
		if (isIdle) {
			playIdle ();
		}

		// crawling
		else if (Input.GetKey (KeyCode.LeftShift)) {
			speed = adjustForBurden (playerStats.crawlSpeed);
			crawlAnimation ();

		}
		// running
		else if (Input.GetKey (KeyCode.LeftControl)) {
			speed = adjustForBurden (playerStats.runSpeed);
			playerStats.isRunning = true;
			runAnimation ();
		}

		// walking
		else {
			speed = adjustForBurden (playerStats.walkSpeed);
			walkAnimation ();
		}

		// when stamina reaches zero, move slowly
		if (playerStats.getStamina() == 0) {
			speed = Mathf.Min (speed, playerStats.crawlSpeed);
		}
        
		/* apply movement to character */
		Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
		Vector3 velocity = inputDirection.normalized;
		a.SetFloat("speed", velocity.magnitude);
        player.Translate(velocity * speed * Time.deltaTime);
    }

	private void crawlAnimation()
	{
		walkAnimation ();
	}

	private void walkAnimation ()
	{

		switch (facing) {
		case Facing.Up:
			a.Play ("back_walk");
			break;
		case Facing.TopRight:
		case Facing.TopLeft:
			a.Play ("backside_walk");
			break;
		case Facing.Right:
		case Facing.Left:
			a.Play ("side_walk");
			break;
		case Facing.BotLeft:
		case Facing.BotRight:
			a.Play ("frontside_walk");
			break;
		case Facing.Down:
			a.Play ("front_walk");
			break;
		default:
			Debug.Log ("Walk animation could not be found");
			break;
		}
	}

	private void setFacing(string input)
	{
		isIdle = false;

		switch (input) {
		case "WAD": // up
		case "W":
			facing = Facing.Up;
			break;
		case "WD": // top-right
			facing = Facing.TopRight;
			break;
		case "D": // right
			facing = Facing.Right;
			break;
		case "SD": // bottom-right
			facing = Facing.BotRight;
			break;
		case "S": // down
		case "ASD":
			facing = Facing.Down;
			break;
		case "AS": // bottom-left
			facing = Facing.BotLeft;
			break;
		case "A": // left
			facing = Facing.Left;
			break;
		case "WA": // top-left
			facing = Facing.TopLeft;
			break;
		default:
			isIdle = true;
			break;
		}
	}

	private void runAnimation ()
	{
		walkAnimation ();
	}


	private void playIdle()
	{
		switch (facing) {
			case Facing.Up:
				a.Play ("player_idle_back");
				break;
			case Facing.TopRight:
			case Facing.TopLeft:
				a.Play ("player_idle_backside");
				break;
			case Facing.Right:
			case Facing.Left:
				a.Play ("player_idle_side");
				break;
			case Facing.BotLeft:
			case Facing.BotRight:
				a.Play ("player_idle_frontside");
				break;
			case Facing.Down:
				a.Play ("player_idle_front");
				break;
			default:
				Debug.Log ("Idle animation could not be found");
				break;
		}
	}

	private string getWASD()
	{
		string str = "";

		if (Input.GetKey (KeyCode.W))
			str += "W";
		if (Input.GetKey (KeyCode.A))
			str += "A";
		if (Input.GetKey (KeyCode.S))
			str += "S";
		if (Input.GetKey (KeyCode.D))
			str += "D";
		return str;
	}


	private float adjustForBurden(float baseSpeed)
	{
		float lowerBound = playerStats.carryCapacity * burdenEffectCutoff;
		if (!isBurdened || (playerStats.inventory.weight <= lowerBound)) {
			return baseSpeed;
		}

		float minSpeedPercent = 0.25f;
		float minSpeed = baseSpeed * minSpeedPercent;
		if (playerStats.inventory.weight > playerStats.carryCapacity) {
			return minSpeed;
		}

		float amount = Mathf.InverseLerp (lowerBound, playerStats.carryCapacity, playerStats.inventory.weight);
		amount = Mathf.InverseLerp (minSpeedPercent, 1, amount);
		amount = Mathf.Clamp(baseSpeed * (1 - amount), minSpeed, baseSpeed);
		return amount;
	}

}

public enum Facing {
	Left,
	Right,
	Up,
	Down,
	TopRight,
	BotRight,
	TopLeft,
	BotLeft
}
