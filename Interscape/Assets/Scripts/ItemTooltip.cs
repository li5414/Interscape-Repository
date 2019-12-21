using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI itemNameText;
	[SerializeField] TextMeshProUGUI itemSlotText;
	[SerializeField] TextMeshProUGUI itemStatsText;

	private void Start ()
	{
		itemNameText = (gameObject.GetComponentsInChildren<TextMeshProUGUI> ()) [0];
		itemSlotText = (gameObject.GetComponentsInChildren<TextMeshProUGUI> ()) [1];
		itemStatsText = (gameObject.GetComponentsInChildren<TextMeshProUGUI> ()) [2];
		gameObject.SetActive (false);
	}

	public void ShowTooltip(Item item)
	{
		itemNameText.text = item.ItemName;
		itemSlotText.text = "Slot?";

		gameObject.SetActive (true);
	}

	public void HideTooltip ()
	{
		gameObject.SetActive (false);
	}
}
