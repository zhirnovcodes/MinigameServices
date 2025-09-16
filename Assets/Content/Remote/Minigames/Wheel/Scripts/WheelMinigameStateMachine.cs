using Cysharp.Threading.Tasks;
using UnityEngine;

public class WheelMinigameStateMachine : MonoBehaviour
{
    [SerializeField] private WheelMinigameWaitState waitState;
    [SerializeField] private WheelMinigameSpinningState spinningState;
    [SerializeField] private WheelMinigameCalibratingState calibratingState;
    [SerializeField] private WheelMinigameFinishState finishState;

    public void Construct(
        WheelMinigameUIPresenter uiPresenter,
        IWheelManualRotationModel manualRotationModel,
        IWheelModel wheelModel,
        IWheelMotorModel motorModel,
        IWheelCalibratorModel calibratorModel,
        WheelTouchHandler handler)
    {
        waitState.Construct(uiPresenter, manualRotationModel, wheelModel, handler);
        
        spinningState.Construct(manualRotationModel, motorModel);
        
        calibratingState.Construct(calibratorModel);
    }

    public async UniTask Play()
    {
        await waitState.WaitForSpin();

        await spinningState.StartSpinning();

        await calibratingState.Calibrate();

        // Show results
        //await finishState.ShowResults();
    }
}
