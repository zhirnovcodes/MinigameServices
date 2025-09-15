using Cysharp.Threading.Tasks;
using UnityEngine;

public class WheelMinigameStateMachine : MonoBehaviour
{
    [SerializeField] private WheelMinigameWaitState waitState;
    [SerializeField] private WheelMinigameSpinningState spinningState;
    [SerializeField] private WheelMinigameCalibratingState calibratingState;
    [SerializeField] private WheelMinigameFinishState finishState;

    public async UniTask Play()
    {
        // Wait for spin input
        await waitState.WaitForSpin();

        // Start spinning with velocity
        float velocity = 0f; // TODO: Get actual velocity
        await spinningState.StartSpinning(velocity);

        // Calibrate the wheel
        await calibratingState.Calibrate();

        // Show results
        await finishState.ShowResults();
    }
}
