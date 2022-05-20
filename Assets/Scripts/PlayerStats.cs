using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour {
    public Inventory inventory;
    public GameObject statsUI;
    public float carryCapacity = 40; // 40 kg start
    public float walkSpeed = 3f;
    public float runSpeed = 8f;
    public float crawlSpeed = 2f;
    public bool isRunning;

    public int skill;
    [Space]
    public Color dangerColor;
    public Color normalColor = Color.white;

    [Space]
    public GameObject cameraShake;

    private int health = 100;
    private int maxHealth = 100;
    private int hunger = 100;
    private int maxHunger = 100;
    private int stamina = 100;
    private int maxStamina = 100;
    private int thirst = 100;
    private int maxThirst = 100;

    private TextMeshProUGUI healthText;
    private TextMeshProUGUI hungerText;
    private TextMeshProUGUI staminaText;
    private TextMeshProUGUI thirstText;

    private bool decreaseStamina;

    // Start is called before the first frame update
    void Start() {
        healthText = statsUI.GetComponentsInChildren<TextMeshProUGUI>()[0];
        hungerText = statsUI.GetComponentsInChildren<TextMeshProUGUI>()[1];
        staminaText = statsUI.GetComponentsInChildren<TextMeshProUGUI>()[2];
        thirstText = statsUI.GetComponentsInChildren<TextMeshProUGUI>()[3];

        StartCoroutine("passiveEffects");
        StartCoroutine("staminaEffects");
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            updateHealth(-10);
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            updateHunger(-10);
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            updateThirst(-10);
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            updateStamina(-10);
        }

        if (isRunning) {
            decreaseStamina = true;
        }
    }


    public void updateHealth(float amount) {
        health = (int)Mathf.Clamp(health + amount, 0, maxHealth);
        updateText(health, maxHealth, healthText);

        if (amount < 0) {
            Instantiate(cameraShake);
        }
    }
    public void updateHunger(float amount) {
        if (hunger > 0 && hunger + amount <= 0) {
            hunger = (int)Mathf.Clamp(hunger + amount, 0, maxHunger);
            StartCoroutine("applyHungerDamage");
        }

        hunger = (int)Mathf.Clamp(hunger + amount, 0, maxHunger);
        updateText(hunger, maxHunger, hungerText);

    }

    public void updateStamina(float amount) {
        stamina = (int)Mathf.Clamp(stamina + amount, 0, maxStamina);
        updateText(stamina, maxStamina, staminaText);
    }

    public void updateThirst(float amount) {
        if (thirst > 0 && thirst + amount <= 0) {
            thirst = (int)Mathf.Clamp(thirst + amount, 0, maxThirst);
            StartCoroutine("applyThirstDamage");
        }

        thirst = (int)Mathf.Clamp(thirst + amount, 0, maxThirst);
        updateText(thirst, maxThirst, thirstText);

    }

    private void updateText(float val, float maxVal, TextMeshProUGUI text) {
        text.text = val + "/" + maxVal;

        if (val <= 20) {
            text.color = dangerColor;
        } else {
            text.color = normalColor;
        }
    }



    private IEnumerator applyHungerDamage() {
        while (health > 0 && hunger <= 0) {
            updateHealth(-10);
            yield return new WaitForSeconds(10);
        }
    }

    private IEnumerator applyThirstDamage() {
        while (health > 0 && thirst <= 0) {
            updateHealth(-10);
            yield return new WaitForSeconds(2.5f);
        }
    }

    private IEnumerator passiveEffects() {
        while (true) {
            yield return new WaitForSeconds(20);
            if (stamina > 0 && health < maxHealth) {
                updateHealth(1);
            }
            if (hunger > 0) {
                updateHunger(-1);
            }
            if (thirst > 0) {
                updateThirst(-1);
            }
        }
    }

    private IEnumerator staminaEffects() {
        while (true) {
            yield return new WaitForSeconds(2);
            if (stamina > 0 && decreaseStamina) {
                updateStamina(-1);
            }
            decreaseStamina = false;
        }
    }



    public int getHealth() {
        return health;
    }
    public int getMaxHealth() {
        return maxHealth;
    }
    public int getHunger() {
        return hunger;
    }
    public int getMaxHunger() {
        return maxHunger;
    }
    public int getStamina() {
        return stamina;
    }
    public int getMaxStamina() {
        return maxStamina;
    }
    public int getThirst() {
        return thirst;
    }
    public int getMaxThirst() {
        return maxThirst;
    }

}
