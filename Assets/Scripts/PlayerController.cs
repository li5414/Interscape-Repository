using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour, IDataPersistence {
    private float BURDEN_EFFECT_CUTOFF = 0.75f;
    public float speed;
    private bool isBurdened = true;
    Animator a;
    private PlayerStats playerStats;
    private Facing facing;
    private bool isIdle;

    /* assign player position and animator */
    void Start() {
        a = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
        facing = Facing.BotRight;
    }

    void FixedUpdate() {

        // get input
        playerStats.isRunning = false;
        string input = getWASD();
        setFacing(input);

        // idle
        if (isIdle) {
            playIdle();
        }

        // crawling
        else if (Input.GetKey(KeyCode.LeftShift)) {
            speed = adjustForBurden(playerStats.crawlSpeed);
            crawlAnimation();

        }
        // running
        else if (Input.GetKey(KeyCode.LeftControl)) {
            speed = adjustForBurden(playerStats.runSpeed);
            playerStats.isRunning = true;
            runAnimation();
        }

        // walking
        else {
            // speed = adjustForBurden(playerStats.walkSpeed);
            speed = playerStats.walkSpeed;
            walkAnimation();
        }

        // when stamina reaches zero, move slowly
        if (playerStats.getStamina() == 0) {
            speed = Mathf.Min(speed, playerStats.crawlSpeed);
        }

        /* apply movement to character */
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        Vector3 velocity = inputDirection.normalized;
        this.transform.Translate(velocity * speed * Time.deltaTime);
    }

    private void crawlAnimation() {
        walkAnimation();
    }

    private void walkAnimation() {
        switch (facing) {
            case Facing.Up:
                a.Play("BackWalk");
                break;
            case Facing.TopRight:
            case Facing.TopLeft:
                a.Play("BackAngledWalk");
                break;
            case Facing.Right:
            case Facing.Left:
                a.Play("SideWalk");
                break;
            case Facing.BotLeft:
            case Facing.BotRight:
                a.Play("FrontAngledWalk");
                break;
            case Facing.Down:
                a.Play("FrontWalk");
                break;
            default:
                Debug.LogError("Walk animation could not be found");
                break;
        }
    }

    private void setFacing(string input) {
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

    private void runAnimation() {
        switch (facing) {
            case Facing.Up:
                a.Play("BackRun");
                break;
            case Facing.TopRight:
            case Facing.TopLeft:
                a.Play("BackAngledRun");
                break;
            case Facing.Right:
            case Facing.Left:
                a.Play("SideRun");
                break;
            case Facing.BotLeft:
            case Facing.BotRight:
                a.Play("FrontAngledRun");
                break;
            case Facing.Down:
                a.Play("FrontRun");
                break;
            default:
                Debug.LogError("Run animation could not be found");
                break;
        }
    }


    private void playIdle() {
        switch (facing) {
            case Facing.Up:
                a.Play("BackIdle");
                break;
            case Facing.TopRight:
            case Facing.TopLeft:
                a.Play("BackAngledIdle");
                break;
            case Facing.Right:
            case Facing.Left:
                a.Play("SideIdle");
                break;
            case Facing.BotLeft:
            case Facing.BotRight:
                a.Play("FrontAngledIdle");
                break;
            case Facing.Down:
                a.Play("FrontIdle");
                break;
            default:
                Debug.LogError("Idle animation could not be found");
                break;
        }
    }

    private string getWASD() {
        string str = "";

        if (Input.GetKey(KeyCode.W))
            str += "W";
        if (Input.GetKey(KeyCode.A))
            str += "A";
        if (Input.GetKey(KeyCode.S))
            str += "S";
        if (Input.GetKey(KeyCode.D))
            str += "D";
        return str;
    }


    private float adjustForBurden(float baseSpeed) {
        float lowerBound = playerStats.carryCapacity * BURDEN_EFFECT_CUTOFF;
        if (!isBurdened || (playerStats.inventory.weight <= lowerBound)) {
            return baseSpeed;
        }

        float minSpeedPercent = 0.25f;
        float minSpeed = baseSpeed * minSpeedPercent;
        if (playerStats.inventory.weight > playerStats.carryCapacity) {
            return minSpeed;
        }

        float amount = Mathf.InverseLerp(lowerBound, playerStats.carryCapacity, playerStats.inventory.weight);
        amount = Mathf.InverseLerp(minSpeedPercent, 1, amount);
        amount = Mathf.Clamp(baseSpeed * (1 - amount), minSpeed, baseSpeed);
        return amount;
    }

    public void LoadData(GameData data) {
        this.transform.position = data.playerData.position;
    }
    public void SaveData(GameData data) {
        data.playerData.position = this.transform.position;
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
