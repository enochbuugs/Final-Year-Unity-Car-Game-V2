using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarControllerOld : MonoBehaviour {

    public float motorTorquePowerAI = 100f; // the car AI variant variable factor for speed.
    public float brakeTorquePowerAI = 50.0f; // how much brake power can it get
    public float steeringAngle;
    public float maxSteerAngle = 30;
    public float brakeAngle = 30f; // determines how cautious the car should be when braking
    public float minSpeed = 0.0f; // the minimum speed of the car
    public float maxSpeed = 100.0f; // the maximum speed of the car
    public float rotSpeed = 1.0f; // how much should it rotate

    public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }

    private float steering;
    public float CurrentSteerAngle { get { return steeringAngle; } }
    private float m_OldRotation;
    [Range(0, 1)] [SerializeField] private float m_SteerHelper;
    [SerializeField] private float m_SteerSensitivity = 0.05f;

    public WheelCollider wheelFrontLeft, wheelFrontRight;
    public WheelCollider wheelRearLeft, wheelRearRight;

    public Transform transformWheelFrontLeft, transformWheelFrontRight;
    public Transform transformWheelRearLeft, transformWheelRearRight;

    Vector3 wheelPos;
    Quaternion wheelRot;
    private Rigidbody m_Rigidbody;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Debug.Log("Steering Angle " + CurrentSteerAngle);
    }

    public void MoveAICar()
    {
        wheelPos = transform.position;
        wheelRot = transform.rotation;

        wheelFrontLeft.GetWorldPose(out wheelPos, out wheelRot);
        wheelFrontRight.GetWorldPose(out wheelPos, out wheelRot);
        wheelRearLeft.GetWorldPose(out wheelPos, out wheelRot);
        wheelRearRight.GetWorldPose(out wheelPos, out wheelRot);

        wheelFrontLeft.motorTorque = motorTorquePowerAI;
        wheelFrontRight.motorTorque = motorTorquePowerAI;
        wheelRearLeft.motorTorque = motorTorquePowerAI;
        wheelRearRight.motorTorque = motorTorquePowerAI;

        //      Steering stuff       //

        steering = Mathf.Clamp(steering, -1, 1);
        steeringAngle = steering * maxSteerAngle;
        wheelFrontLeft.steerAngle = steeringAngle;
        wheelFrontRight.steerAngle = steeringAngle;

        SteeringHelp();
        WheelMeshPosRot();
    }

    void WheelMeshPosRot()
    {
        transformWheelFrontLeft.transform.position = wheelPos;
        transformWheelFrontRight.transform.position = wheelPos;
        transformWheelRearLeft.transform.position = wheelPos;
        transformWheelRearRight.transform.position = wheelPos;

        transformWheelFrontLeft.transform.rotation = wheelRot;
        transformWheelFrontRight.transform.rotation = wheelRot;
        transformWheelRearLeft.transform.rotation = wheelRot;
        transformWheelRearRight.transform.rotation = wheelRot;
    }

    void SteeringHelp()
    {
        WheelHit wheelHit;

        wheelFrontLeft.GetGroundHit(out wheelHit);
        wheelFrontRight.GetGroundHit(out wheelHit);
        wheelRearLeft.GetGroundHit(out wheelHit);
        wheelRearRight.GetGroundHit(out wheelHit);

        if (wheelHit.normal == Vector3.zero)
        {
            return;
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
        }
        m_OldRotation = transform.eulerAngles.y;
    }

    public void UpdateWheelMotions()
    {
        // This method updates all the wheel colliders and the wheel mesh in real time as they ride together.
        UpdateWheelMotion(wheelFrontLeft, transformWheelFrontLeft);
        UpdateWheelMotion(wheelFrontRight, transformWheelFrontRight);
        UpdateWheelMotion(wheelRearLeft, transformWheelRearLeft);
        UpdateWheelMotion(wheelRearRight, transformWheelRearRight);
    }


    void UpdateWheelMotion(WheelCollider wheelC, Transform wheelT)
    {
        wheelPos = transform.position;
        wheelRot = transform.rotation;

        // This gets the information of the wheel collider and transform
        // Returns the infromation with GetWorldPos
        // To align the wheels correctly to spin the right way 
        wheelC.GetWorldPose(out wheelPos, out wheelRot);

        // Then we pass the information into our method parameters...
        // Both position and rotation.
        wheelT.transform.position = wheelPos;
        wheelT.transform.rotation = wheelRot;
    }

}
