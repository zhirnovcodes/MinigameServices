using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class WheelMinigameCalibratingState : MonoBehaviour
{
    private IWheelCalibratorModel CalibratorModel;

    public void Construct(IWheelCalibratorModel calibratorModel)
    {
        CalibratorModel = calibratorModel;
    }

    public async UniTask Calibrate()
    {
        CalibratorModel.Enable();
        var token = this.GetCancellationTokenOnDestroy();
        while (CalibratorModel.IsCalibrated() == false)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
        CalibratorModel.Disable();
    }
}
