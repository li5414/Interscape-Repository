using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI itemNameText;
	[SerializeField] TextMeshProUGUI itemDescriptionText;
	[SerializeField] TextMeshProUGUI itemWeightText;
	[SerializeField] TextMeshProUGUI itemQualityText;
	[SerializeField] TextMeshProUGUI itemDamageText;

	private void Start ()
	{
		itemNameText = gameObject.GetComponentsInChildren<TextMeshProUGUI> () [0];
		itemDescriptionText = gameObject.GetComponentsInChildren<TextMeshProUGUI> () [1];
		itemWeightText = gameObject.GetComponentsInChildren<TextMeshProUGUI> () [2];
		itemQualityText = gameObject.GetComponentsInChildren<TextMeshProUGUI> () [3];
		itemDamageText = gameObject.GetComponentsInChildren<TextMeshProUGUI> () [4];
		gameObject.SetActive (false);
	}

	public void ShowTooltip(Item item)
	{
		if (item != null) {
			itemNameText.text = item.itemName;
			itemDescriptionText.text = item.description.Replace ("\\n", "\n");
			itemWeightText.text = "Weight: " + item.weight + "kg";
			if (item is Tool) {
				itemQualityText.text = "Quality: " + ((Tool)item).getQuality() + " (" + ((Tool)item).getDurability() + " hits)";
				itemDamageText.text = "Damage: " + ((Tool)item).getDamage ();
				itemQualityText.enabled = true;
				itemDamageText.enabled = true;
			} else {
				itemQualityText.enabled = false;
				itemDamageText.enabled = false;
			}

			gameObject.SetActive (true);
		}
	}

	public void HideTooltip ()
	{
		gameObject.SetActive (false);
	}
}
