using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class WheelMinigameSpinningState : MonoBehaviour
{
    public Vector2 SpinTime; // x=min, y=max
    public Vector2 SlowDownTime; // x=min, y=max
    public float VelocityMultiplier = 1;

    private IWheelManualRotationModel ManualRotationModel;
    private IWheelMotorModel MotorModel;

    public void Construct(
        IWheelManualRotationModel manualRotationModel,
        IWheelMotorModel motorModel)
    {
        ManualRotationModel = manualRotationModel;
        MotorModel = motorModel;
    }

    public async UniTask StartSpinning()
    {
        var token = this.GetCancellationTokenOnDestroy();

        float initialAngularVelocity = - ManualRotationModel.GetAngVelocity();

        MotorModel.Enable();
        MotorModel.SetMotorTargetVelocity(initialAngularVelocity * VelocityMultiplier);
        MotorModel.SetMotorForce(50f);

        float spinDuration = Random.Range(SpinTime.x, SpinTime.y);
        await UniTask.Delay(System.TimeSpan.FromSeconds(spinDuration), DelayType.DeltaTime, PlayerLoopTiming.Update, token);

        MotorModel.SetMotorForce(0f);

        float slowDownDuration = Mathf.Max(0f, Random.Range(SlowDownTime.x, SlowDownTime.y));
        float elapsed = 0f;
        float startVelocity = initialAngularVelocity;
        while (elapsed < slowDownDuration)
        {
            elapsed += Time.deltaTime;
            float t = slowDownDuration <= 0.0001f ? 1f : Mathf.Clamp01(elapsed / slowDownDuration);
            float current = Mathf.Lerp(startVelocity, 0f, t);
            MotorModel.SetMotorTargetVelocity(current);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        MotorModel.ForceStop();
        MotorModel.Disable();
    }
}
