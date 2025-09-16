using UnityEngine;

public interface IWheelManualRotationModel
{
    void Enable();
    void Disable();

    float GetForceValue();
    float GetAngVelocity();
}

public class WheelManualRotationModel : MonoBehaviour, IWheelManualRotationModel
{
    public float MaxAngVelocityDeg = 30;
    public Rigidbody Rigidbody;
    public WheelTouchHandler Handler;
    public float AngVel;

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

    public float GetForceValue()
    {
        return Handler.GetTouchDirection().x;
    }

    private void FixedUpdate()
    {
        if (Handler.GetIsTouchingWheel() == false)
        {
            return;
        }

        var x = Handler.GetTouchDirection().x;

        AngVel = GetAngVelocity();

        if (x <= 0)
        {
            return;
        }

        Vector3 angularVelocity = Rigidbody.angularVelocity;
        angularVelocity.y = - x * MaxAngVelocityDeg * Mathf.Deg2Rad; // Add some angular velocity
        Rigidbody.angularVelocity = angularVelocity;
    }

    public float GetAngVelocity()
    {
        return Rigidbody.angularVelocity.y * Mathf.Rad2Deg;
    }
}
