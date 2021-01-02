using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
	public Inventory inventory;
	public GameObject statsUI;
	public float carryCapacity = 40; // 40 kg start
	public float walkSpeed = 0.08f;
	public float runSpeed = 0.2f;
	public float crawlSpeed = 0.04f;

	public int skill;
	public Color dangerColor;
	public Color normalColor = Color.white;

	[Space]
	public GameObject cameraShake;

	private float health = 100;
	private float maxHealth = 100;
	private float hunger = 100;
	private float maxHunger = 100;
	private float stamina = 100;
	private float maxStamina = 100;
	private float thirst = 100;
	private float maxThirst = 100;


	private TextMeshProUGUI healthText;
	private TextMeshProUGUI hungerText;
	private TextMeshProUGUI staminaText;
	private TextMeshProUGUI thirstText;

	// Start is called before the first frame update
	void Start()
    {
		healthText = statsUI.GetComponentsInChildren<TextMeshProUGUI> () [0];
		hungerText = statsUI.GetComponentsInChildren<TextMeshProUGUI> () [1];
		staminaText = statsUI.GetComponentsInChildren<TextMeshProUGUI> () [2];
		thirstText = statsUI.GetComponentsInChildren<TextMeshProUGUI> () [3];
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
			updateHealth (-10);
		}
    }







	public void updateHealth(float amount)
	{
		health = Mathf.Clamp(health + amount, 0, maxHealth);
		updateText (health, maxHealth, healthText);
		Instantiate (cameraShake);
	}
	public void updateHunger (float amount)
	{
		hunger = Mathf.Clamp (hunger + amount, 0, maxHunger);
		updateText (hunger, maxHunger, hungerText);
	}
	public void updateStamina (float amount)
	{
		stamina = Mathf.Clamp (stamina + amount, 0, maxStamina);
		updateText (stamina, maxStamina, staminaText);
	}
	public void updateThirst (float amount)
	{
		thirst = Mathf.Clamp (thirst + amount, 0, maxThirst);
		updateText (thirst, maxThirst, thirstText);
	}

	private void updateText (float val, float maxVal, TextMeshProUGUI text)
	{
		text.text = val + "/" + maxVal;

		if (val <= 20) {
			text.color = dangerColor;
		} else {
			text.color = normalColor;
		}
	}





	public float getHealth()
	{
		return health;
	}
	public float getMaxHealth ()
	{
		return maxHealth;
	}
	public float getHunger ()
	{
		return hunger;
	}
	public float getMaxHunger ()
	{
		return maxHunger;
	}
	public float getStamina ()
	{
		return stamina;
	}
	public float getMaxStamina ()
	{
		return maxStamina;
	}
	public float getThirst ()
	{
		return thirst;
	}
	public float getMaxThirst ()
	{
		return maxThirst;
	}

}
