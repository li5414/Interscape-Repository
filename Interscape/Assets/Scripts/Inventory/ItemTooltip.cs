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

	private void Start ()
	{
		itemNameText = gameObject.GetComponentsInChildren<TextMeshProUGUI> () [0];
		itemDescriptionText = gameObject.GetComponentsInChildren<TextMeshProUGUI> () [1];
		itemWeightText = gameObject.GetComponentsInChildren<TextMeshProUGUI> () [2];
		itemQualityText = gameObject.GetComponentsInChildren<TextMeshProUGUI> () [3];
		gameObject.SetActive (false);
	}

	public void ShowTooltip(Item item)
	{
		if (item) {
			itemNameText.text = item.itemName;
			itemDescriptionText.text = item.description.Replace ("\\n", "\n");
			itemWeightText.text = "Weight: " + item.weight + "kg";
			if (item is Tool) {
				itemQualityText.text = "Quality: " + ((Tool)item).quality;
				itemQualityText.enabled = true;
			} else {
				itemQualityText.enabled = false;
			}

			gameObject.SetActive (true);
		}
	}

	public void HideTooltip ()
	{
		gameObject.SetActive (false);
	}
}
