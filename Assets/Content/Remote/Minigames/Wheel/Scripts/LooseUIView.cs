using UnityEngine;
using TMPro;

public class LooseUIView : MonoBehaviour
{
	public Transform Root;
	public TextMeshProUGUI CountText;
	public GameObject NoneText;
	public TextMeshProUGUI OperationText;
	public GameObject PoopImage;
	public GameObject DiamondImage;
	public GameObject CashImage;

	public void Enable()
	{
		Root.gameObject.SetActive(true);
	}

	public void Disable()
	{
		Root.gameObject.SetActive(false);
	}

	public void SetLoseData(WheelGamePenaltyData data)
	{
		// Reset visuals
		NoneText.SetActive(false);
		PoopImage.SetActive(false);
		DiamondImage.SetActive(false);
		CashImage.SetActive(false);
	    OperationText.text = string.Empty;
		CountText.text = string.Empty;

		// Resource icon
		switch (data.Resource)
		{
			case WheelGamePenaltyResource.Diamonds:
				DiamondImage.SetActive(true);
				break;
			case WheelGamePenaltyResource.Cash:
				CashImage.SetActive(true);
				break;
		}

		// Penalty type
		switch (data.Penalty)
		{
			case WheelGamePenaltyType.Nothing:
				NoneText.SetActive(true);
				PoopImage.SetActive(true);
				break;
			case WheelGamePenaltyType.LoseAll:
				OperationText.text = "Lose All!!!";
				break;
			case WheelGamePenaltyType.LoseHalf:
				OperationText.text = "Lose";
				CountText.text = "50%";
				break;
			case WheelGamePenaltyType.LoseAmount:
				if (OperationText != null) OperationText.text = "Lose";
				if (CountText != null) CountText.text = data.Amount.ToString();
				break;
		}
	}
}


