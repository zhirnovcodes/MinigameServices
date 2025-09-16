using UnityEngine;

public interface IWheelManualRotationModel
{
    void Enable();
    void Disable();
}

public class WheelManualRotationModel : MonoBehaviour, IWheelManualRotationModel
{
    public float MaxAngVelocityDeg = 30;
    public Rigidbody Rigidbody;
    public WheelTouchHandler Handler;

    public void Enable()
    {
        enabled = true;
        Handler.enabled = true;
    }

    public void Disable()
    {
        enabled = false;
        Handler.enabled = false;
    }

    public float GetAngularSpeed()
    {
        return Rigidbody.angularVelocity.y;
    }

    private void FixedUpdate()
    {
        if (Handler.GetIsTouchingWheel() == false)
        {
            return;
        }

        var x = Handler.GetTouchDirection().x;

        if (x <= 0)
        {
            return;
        }

        Vector3 angularVelocity = Rigidbody.angularVelocity;
        angularVelocity.y = - x * MaxAngVelocityDeg * Mathf.Deg2Rad; // Add some angular velocity
        Rigidbody.angularVelocity = angularVelocity;
    }
}
