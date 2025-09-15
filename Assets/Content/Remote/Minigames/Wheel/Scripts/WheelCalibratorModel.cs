using UnityEngine;
using Cysharp.Threading.Tasks;

public interface IWheelCalibratorModel
{
    UniTask Calibrate();
    void Enable();
    void Disable();
}

public class WheelCalibratorModel : MonoBehaviour, IWheelCalibratorModel
{
    public WheelModel wheelModel;
    public Rigidbody rigidbody;

    public async UniTask Calibrate()
    {
        int currentSectionId = wheelModel.GetCurrentSectionId();
        Transform targetPlaceholder = wheelModel.GetPlaceholderTransform(currentSectionId);
        
        if (targetPlaceholder == null)
            return;

        // Calculate the direction from wheel to the target placeholder
        Vector3 directionToTarget = (targetPlaceholder.position - transform.position).normalized;
        
        // Calculate the target rotation to face the placeholder
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        
        // Smoothly rotate towards the target
        float rotationSpeed = 90f; // degrees per second
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
        float rotationTime = angleDifference / rotationSpeed;
        
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        
        while (elapsedTime < rotationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rotationTime;
            
            // Use rigidbody to rotate smoothly
            Quaternion currentTarget = Quaternion.Slerp(startRotation, targetRotation, t);
            rigidbody.MoveRotation(currentTarget);
            
            await UniTask.Yield();
        }
        
        // Ensure final rotation is set
        rigidbody.MoveRotation(targetRotation);
    }

    public void Enable()
    {
        enabled = true;
    }

    public void Disable()
    {
        enabled = false;
    }
}
