using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class WinCardAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float FlyInTime = 1f;
    public float ShowOffTime = 1.5f;
    public float DepthChange = 2f;
    public Transform Root;

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
        var target = Root;
        target.rotation = Quaternion.identity;

        var seq = DOTween.Sequence()
            .SetTarget(this)
            .SetAutoKill(true);

        // Phase 1: Fly in - move forward by DepthChange on Z axis
        seq.Append(transform.DOMoveZ(transform.position.z + DepthChange, Mathf.Max(0f, FlyInTime))
            .SetEase(Ease.OutQuart));

        // Phase 2: Show off - rotate around Y axis 30 degrees left and right
        seq.Append(target.DORotate(new Vector3(0f, 30f, 0f), Mathf.Max(0f, ShowOffTime * 0.5f), RotateMode.Fast)
            .SetEase(Ease.InOutSine));
        seq.Append(target.DORotate(new Vector3(0f, -30f, 0f), Mathf.Max(0f, ShowOffTime * 0.5f), RotateMode.Fast)
            .SetEase(Ease.InOutSine));

        await seq.AsyncWaitForCompletion();
    }

    private void OnDestroy()
    {
        // no cached tweens to kill
    }
}
