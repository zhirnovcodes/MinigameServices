using UnityEngine;
using DG.Tweening;

public class WheelMinigameWaitingView : MonoBehaviour
{
	public Transform Arrow;
	public Transform Root;

	Tween scaleTween;

	public void Enable()
	{
		Root.gameObject.SetActive(true);
	}

	public void StartAnimation()
	{
		scaleTween?.Kill();
		Arrow.localScale = Vector3.one;
		scaleTween = Arrow
			.DOScale(1.15f, 1f)
			.SetLoops(-1, LoopType.Yoyo)
			.SetEase(Ease.InOutSine)
			.SetTarget(this);
	}

	public void Disable()
	{
		scaleTween?.Kill();
		scaleTween = null;
		Root.gameObject.SetActive(false);
	}
}


