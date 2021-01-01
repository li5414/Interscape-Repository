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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
		playerStats = GetComponent<PlayerStats> ();
    }

    void Update() {
		SpriteRenderer spriteR = hair.GetComponent<SpriteRenderer>();

		/* test whether to face right or left */
		float move = Input.GetAxis("Horizontal");
        if (move > 0 && !right) {
            Flip();
        }
        else {
            if (move < 0 && right)
                Flip();
        }

		

		/* test which keys are being pressed */
        bool is_shift_key = Input.GetKey(KeyCode.LeftShift);
		bool is_ctrl_key = Input.GetKey (KeyCode.LeftControl);
		a.SetBool("is_shift_key", is_shift_key);
		if (is_shift_key)
			speed = adjustForBurden(playerStats.crawlSpeed);
		else if (is_ctrl_key) {
			speed = adjustForBurden(playerStats.runSpeed);
		}
		else {
			speed = adjustForBurden(playerStats.walkSpeed);
		}
		
		bool is_w_key = Input.GetKey(KeyCode.W);	
		bool is_s_key = Input.GetKey(KeyCode.S);
		bool is_d_key = Input.GetKey(KeyCode.D);
		bool is_a_key = Input.GetKey(KeyCode.A);
		bool is_ad_key = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A));
		bool is_awd_key = (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D));
		bool is_asd_key = (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D));
		
		/* prioritise diagonal movement */
		if (is_asd_key) {
			a.SetBool("is_asd_key", is_asd_key);
			a.SetBool("is_a_key", false);
			a.SetBool("is_s_key", false);
			a.SetBool("is_d_key", false);
			a.SetBool("is_w_key", false);
			spriteR.sprite = hairFrontside;
		}
        else if (is_awd_key) {
        	a.SetBool("is_awd_key", is_awd_key);
        	a.SetBool("is_a_key", false);
        	a.SetBool("is_w_key", false);
        	a.SetBool("is_d_key", false);
        	a.SetBool("is_s_key", false);
			spriteR.sprite = hairBackside;
		}
        else if (is_s_key) {
        	a.SetBool("is_s_key", is_s_key);
        	a.SetBool("is_a_key", false);
        	a.SetBool("is_w_key", false);
        	a.SetBool("is_d_key", false);
        	a.SetBool("is_asd_key", false);
        	a.SetBool("is_awd_key", false);
			spriteR.sprite = hairFront;
		}
        else if (is_d_key) {
        	a.SetBool("is_d_key", is_d_key);
        	a.SetBool("is_a_key", false);
        	a.SetBool("is_s_key", false);
        	a.SetBool("is_w_key", false);
        	a.SetBool("is_asd_key", false);
        	a.SetBool("is_awd_key", false);
			spriteR.sprite = hairSide;
		}
        else if (is_a_key) {
        	a.SetBool("is_a_key", is_a_key);
        	a.SetBool("is_w_key", false);
        	a.SetBool("is_s_key", false);
        	a.SetBool("is_d_key", false);
        	a.SetBool("is_asd_key", false);
        	a.SetBool("is_awd_key", false);
			spriteR.sprite = hairSide;
		}
        else if (is_w_key) {
        	a.SetBool("is_w_key", is_w_key);
        	a.SetBool("is_a_key", false);
        	a.SetBool("is_s_key", false);
        	a.SetBool("is_d_key", false);
            a.SetBool("is_asd_key", false);
            a.SetBool("is_awd_key", false);
			spriteR.sprite = hairBack;
		}
        else {
        	a.SetBool("is_a_key", false);
        	a.SetBool("is_w_key", false);
        	a.SetBool("is_d_key", false);
        	a.SetBool("is_s_key", false);
        	a.SetBool("is_awd_key", false);
        	a.SetBool("is_asd_key", false);
			spriteR.sprite = hairFrontside;
		}
        
		/* apply movement to character */
		Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
		Vector3 velocity = inputDirection.normalized;
		a.SetFloat("speed", velocity.magnitude);
        player.Translate(velocity * speed * Time.deltaTime);
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


	/* function to flip object */
    void Flip() {
        right = !right;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
