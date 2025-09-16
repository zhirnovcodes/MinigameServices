using UnityEngine;

public interface IWheelCalibratorModel
{
    bool IsCalibrated();
    void Enable();
    void Disable();
}

public class WheelCalibratorModel : MonoBehaviour, IWheelCalibratorModel
{
    public WheelModel wheelModel;
    public Rigidbody rigidbody;
    public Transform WheelTransform;
    public Transform Clicker;

    [Header("Calibration Settings")]
    public float rotationSpeedDegreesPerSecond = 90f;
    public float angleToleranceDegrees = 1f;

    private bool isCalibrated;

    public bool IsCalibrated()
    {
        return isCalibrated;
    }

    private void OnEnable()
    {
        isCalibrated = false;
    }

    private void FixedUpdate()
    {
        // Compute signed angle on the wheel plane from current winning placeholder to clicker direction
        int currentSectionId = wheelModel.GetCurrentSectionId();
        Transform targetPlaceholder = wheelModel.GetPlaceholderTransform(currentSectionId);

        Vector3 center = WheelTransform.position;
        Vector3 planeNormal = WheelTransform.up;

        Vector3 targetDir = Vector3.ProjectOnPlane(targetPlaceholder.position - center, planeNormal).normalized;
        Vector3 clickerDir = Vector3.ProjectOnPlane(Clicker.position - center, planeNormal).normalized;

        float signedAngle = Vector3.SignedAngle(targetDir, clickerDir, planeNormal);

        float absAngle = Mathf.Abs(signedAngle);
        if (absAngle <= angleToleranceDegrees)
        {
            isCalibrated = true;
            return;
        }

        isCalibrated = false;

        float step = Mathf.Sign(signedAngle) * Mathf.Min(absAngle, rotationSpeedDegreesPerSecond * Time.fixedDeltaTime);

        // Apply rotation around local Y (wheel's up axis) in LOCAL coordinates
        Quaternion localStep = Quaternion.AngleAxis(step, Vector3.up);
        Transform wheel = WheelTransform;

        Quaternion newLocal = wheel.localRotation * localStep;
        Quaternion newWorld = wheel.parent != null ? wheel.parent.rotation * newLocal : newLocal;
        rigidbody.MoveRotation(newWorld);
    }

    public void Enable()
    {
        enabled = true;
        rigidbody.angularDrag = 1f;
        rigidbody.angularVelocity = Vector3.zero;
    }

    public void Disable()
    {
        enabled = false;
    }
}
