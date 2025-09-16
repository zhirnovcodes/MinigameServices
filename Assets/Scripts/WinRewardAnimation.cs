using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class WinRewardAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public Transform Root;
    public float DepthChange = 2f;
    public float StartRotationSpeed = 360f;
    public float AnimationTime = 2f;

    

    

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public async UniTask PlayAnimation()
    {
        // Start from current position
        Root.rotation = Quaternion.identity;

        float duration = Mathf.Max(0f, AnimationTime);
        float degrees = Mathf.Max(0f, StartRotationSpeed) * duration;

        var seq = DOTween.Sequence()
            .SetTarget(this)
            .SetAutoKill(true);

        seq.Append(transform.DOMoveZ(transform.position.z + DepthChange, duration)
            .SetEase(Ease.OutQuart));

        seq.Join(Root.DORotate(new Vector3(0f, degrees, 0f), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuart));

        await seq.AsyncWaitForCompletion();
    }

    private void OnDestroy()
    {
        // no cached tweens to kill
    }
}
