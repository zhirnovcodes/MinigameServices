using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameplayResultView : MonoBehaviour
{
    [SerializeField] private GameObject ResultUI;
    [SerializeField] private GameObject RewardAnimation;
    [SerializeField] private GameObject PenaltyAnimation;
    [SerializeField] private GameObject TimeIsUpAnimation;

    
    public void SetResult(MemoryGameResultData resultData)
    {
        // Update UI elements based on result data
        // This would typically update text, images, etc. based on the result
    }
    
    public async UniTask PlayRewardAnimation()
    {
        RewardAnimation.SetActive(true);
        // Play reward animation (e.g., confetti, coins, etc.)
        await UniTask.Delay(2000); // Simulate animation duration
        RewardAnimation.SetActive(false);
    }
    
    public async UniTask PlayPenaltyAnimation()
    {
        PenaltyAnimation.SetActive(true);
        // Play penalty animation (e.g., red flash, negative effects, etc.)
        await UniTask.Delay(1500); // Simulate animation duration
        PenaltyAnimation.SetActive(false);
    }
    
    public async UniTask PlayTimeIsUpAnimation()
    {
        TimeIsUpAnimation.SetActive(true);
        // Play time's up animation (e.g., clock animation, warning, etc.)
        await UniTask.Delay(2000); // Simulate animation duration
        TimeIsUpAnimation.SetActive(false);
    }
    
    public void Enable()
    {
        ResultUI.SetActive(true);
    }
    
    public void Disable()
    {
        ResultUI.SetActive(false);
    }
}
