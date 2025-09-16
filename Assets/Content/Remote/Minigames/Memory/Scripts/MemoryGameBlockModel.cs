using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class MemoryGameBlockModel : MonoBehaviour
{
    public event Action<int> Clicked = (index) => { };
    [SerializeField] private Collider Collider;
    [SerializeField] private int Index;
    [SerializeField] private float FlipDuration = 0.25f;

    private Tweener flipShowTween;
    private Tweener flipHideTween;

    private void Awake()
    {
        // Pre-create tweens (garbage-free), keep them paused and reusable
        flipShowTween = transform
            .DOLocalRotate(new Vector3(0f, 180f, 0f), FlipDuration, RotateMode.Fast)
            .SetAutoKill(false)
            .Pause();

        flipHideTween = transform
            .DOLocalRotate(new Vector3(0f, 0f, 0f), FlipDuration, RotateMode.Fast)
            .SetAutoKill(false)
            .Pause();
    }

    public void SetIndex(int index)
    {
        Index = index;
    }

    public int GetIndex()
    {
        return Index;
    }

    public void SetInteractive()
    {
        Collider.enabled = true;
    }

    public void SetNonInteractive()
    {
        Collider.enabled = false;
    }

    public void HandleClicked()
    {
        Clicked(Index);
    }

    public void SetResult(MemoryGameResultData data)
    {
        // Hook for setting visual/content based on result data
    }

    public async UniTask PlayFlipShowAnimation()
    {
        // Ensure correct start state, pause opposite tween, then replay
        flipHideTween.Pause();
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        flipShowTween.Restart();
        await flipShowTween.AsyncWaitForCompletion();
    }

    public async UniTask PlayFlipHideAnimation()
    {
        // Ensure correct start state, pause opposite tween, then replay
        flipShowTween.Pause();
        transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        flipHideTween.Restart();
        await flipHideTween.AsyncWaitForCompletion();
    }
}


