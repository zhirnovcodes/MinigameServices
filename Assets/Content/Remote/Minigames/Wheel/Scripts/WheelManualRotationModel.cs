using UnityEngine;

public interface IWheelManualRotationModel
{
    void Enable();
    void Disable();
    float GetAngularSpeed();
    void AddAngularForce();
}

public class WheelManualRotationModel : MonoBehaviour, IWheelManualRotationModel
{
    public Rigidbody rigidbody;

    public void Enable()
    {
        enabled = true;
    }

    public void Disable()
    {
        enabled = false;
    }

    public float GetAngularSpeed()
    {
        return rigidbody.angularVelocity.magnitude;
    }

    public void AddAngularForce()
    {
        Vector3 angularVelocity = rigidbody.angularVelocity;
        angularVelocity.y += 1f; // Add some angular velocity
        rigidbody.angularVelocity = angularVelocity;
    }
}
