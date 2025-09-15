using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public interface IWheelAppearAnimationModel
{
    void Enable();
    void Disable();
    UniTask PlayAnimation();
}

public class WheelAppearAnimationModel : MonoBehaviour, IWheelAppearAnimationModel
{
    private Tween rotationTween;

    private void Awake()
    {
        // Create the tween once without allocations
        rotationTween = transform.DORotate(new Vector3(90f, 0f, 0f), 1f, RotateMode.FastBeyond360)
            .SetTarget(this)
            .SetEase(Ease.OutBounce)
            .SetAutoKill(false);
        
        rotationTween.Pause();
    }

    public void Enable()
    {
        enabled = true;
    }

    public void Disable()
    {
        enabled = false;
    }

    public async UniTask PlayAnimation()
    {
        // Reset rotation to starting position
        transform.rotation = Quaternion.identity;
        
        // Restart the tween
        rotationTween.Restart();
        
        // Wait for completion
        await rotationTween.AsyncWaitForCompletion();
    }
}
