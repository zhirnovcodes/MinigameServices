using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class WheelMinigameWaitState : MonoBehaviour
{
    public float MinAngVelocity = 10;
    public float MaxAngVelocity = 20;

    private WheelMinigameUIPresenter UIPresenter;
    private IWheelManualRotationModel ManualRotationModel;
    private IWheelModel WheelModel;
    private WheelTouchHandler Handler;

    public void Construct(
        WheelMinigameUIPresenter uiPresenter,
        IWheelManualRotationModel manualRotationModel,
        IWheelModel wheelModel,
        WheelTouchHandler handler)
    {
        UIPresenter = uiPresenter;
        ManualRotationModel = manualRotationModel;
        WheelModel = wheelModel;
        Handler = handler;
    }

    public async UniTask WaitForSpin()
    {
        UIPresenter.EnableWaitingView();
        ManualRotationModel.Enable();

        var token = this.GetCancellationTokenOnDestroy();
        while (Mathf.Abs(ManualRotationModel.GetAngVelocity()) <= MinAngVelocity ||
            (Handler.GetIsTouchingWheel() && 
            Mathf.Abs(ManualRotationModel.GetAngVelocity()) <= MaxAngVelocity))
        {
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        UIPresenter.DisableWaitingView();
        ManualRotationModel.Disable();
    }
}
