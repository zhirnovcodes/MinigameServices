using UnityEngine;
using TMPro;

public class WinUIView : MonoBehaviour
{
	public Transform Root;
	public TextMeshProUGUI CountText;

	public void Enable()
	{
		Root.gameObject.SetActive(true);
	}

	public void SetWinData(WheelGameRewardData data)
	{
		int total = data.Cash + data.Diamonds + (data.Cards.Count);
		CountText.text = total.ToString();
	}

	public void Disable()
	{
		Root.gameObject.SetActive(false);
	}
}


