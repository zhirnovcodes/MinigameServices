using UnityEngine;

public interface IWheelMotorModel
{
    float GetAngularSpeed();
    void SetMotorTargetVelocity(float velocity);
    void SetMotorForce(float force);
    void Enable();
    void Disable();
}

public class WheelMotorModel : MonoBehaviour, IWheelMotorModel
{
    public HingeJoint hingeJoint;

    public float GetAngularSpeed()
    {
        return hingeJoint.velocity;
    }

    public void SetMotorTargetVelocity(float velocity)
    {
        JointMotor motor = hingeJoint.motor;
        motor.targetVelocity = velocity;
        hingeJoint.motor = motor;
    }

    public void SetMotorForce(float force)
    {
        JointMotor motor = hingeJoint.motor;
        motor.force = force;
        hingeJoint.motor = motor;
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
