using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficAIBasic : MonoBehaviour
{

    // TESTING VARIABLES TO GET ACCURACY OF THE AI CAR 
    // TO DO LATER..
    // ADD THE WHEELCOLLIDERS TO AI CARS (DONE)
    // ADD AVOIDANCE BEHAVIOUR
    // SECONDLY GET IT TO PATHFIND AFTER TESTING THIS CODE THROUGHLY
    //.........

    [Header("AI Follow Target")]
    public Transform target; // the target the AI will follow
    private Vector3 lookAtTarget; // holds the info to look at the target we should follow...
    private Vector3 distance; // the distance between the player and target it has to follow..
    private Vector3 wheelPos;
    private Quaternion wheelRot;

    [Header("AI Avoidance Properties")]
    public Transform obstacle;


    [Header("Rigidbody/Wheelcolliders")]
    public Rigidbody rb;

    [Header("Wheel Transforms")]
    public Transform transformWheelFrontLeft;
    public Transform transformWheelFrontRight;
    public Transform transformWheelRearLeft;
    public Transform transformWheelRearRight;

    [Header("Wheelcolliders")]
    public WheelCollider wheelFrontLeft;
    public WheelCollider wheelFrontRight;
    public WheelCollider wheelRearLeft;
    public WheelCollider wheelRearRight;

    [Header("AI Car Properties")]
    public float setMaxSpeed;
    public float setMinimumSpeed;
    public float brakeAngle; // determines how cautious the car should be when braking
    public float rotSpeed; // how much should it rotate
    public float torquePower; // the car AI variant variable factor for speed.
    public float normalTorquePower;
    public float brakeTorquePower; // how much brake power can it get
    public float normalBrakeTorquePower;
    public float steeringAngle;
    public float maxSteerAngle = 30;

    private float steering;
    private float maximumSpeed;
    private float minimumSpeed;
    public float currentSpeed { get { return rb.velocity.magnitude * 2.23693629f; } }
    public float CurrentSteerAngle { get { return steeringAngle; } }
    private float m_OldRotation;

    [Range(0, 1)] [SerializeField] private float m_SteerHelper;
    [SerializeField] private float m_SteerSensitivity = 0.05f;
    



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.9f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        MoveCar();
        Avoidance();
        UpdateWheelMotions();
        GoToTarget();
    }

    void GoToTarget()
    {
        lookAtTarget = new Vector3(target.position.x, transform.position.y, target.transform.position.z);
        distance = lookAtTarget - this.transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(distance, transform.up), Time.deltaTime * rotSpeed);
    }

    void MoveCar()
    {
        minimumSpeed = setMinimumSpeed;
        maximumSpeed = setMaxSpeed;

        // if the cars speed is greater or equal to the max speed set
        // then dont apply anymore torque power
        if (currentSpeed >= setMaxSpeed)
        {
            torquePower = 0;
            brakeTorquePower = 0;
        }
        else
        {
            torquePower = normalTorquePower;
            brakeTorquePower = normalBrakeTorquePower;
        }

        if ((currentSpeed >= setMaxSpeed) || (currentSpeed <= setMaxSpeed) && (Vector3.Angle(target.forward, transform.forward) > brakeAngle))
        {
            torquePower = 0;
            brakeTorquePower = normalBrakeTorquePower;
        }


        wheelPos = transform.position;
        wheelRot = transform.rotation;

        wheelFrontLeft.GetWorldPose(out wheelPos, out wheelRot);
        wheelFrontRight.GetWorldPose(out wheelPos, out wheelRot);
        wheelRearLeft.GetWorldPose(out wheelPos, out wheelRot);
        wheelRearRight.GetWorldPose(out wheelPos, out wheelRot);

        wheelFrontLeft.motorTorque = torquePower;
        wheelFrontRight.motorTorque = torquePower;
        wheelRearLeft.motorTorque = torquePower;
        wheelRearRight.motorTorque = torquePower;

        //      Steering stuff       //

        steering = Mathf.Clamp(steering, -1, 1);
        steeringAngle = steering * maxSteerAngle;
        wheelFrontLeft.steerAngle = steeringAngle;
        wheelFrontRight.steerAngle = steeringAngle;

        //SteeringHelp();
        WheelMeshPosRot();
    }

    void Avoidance()
    {
        float raycastLength = 10f;
        float raycastAngle = 20f;

        RaycastHit hit;
        Ray centreRay = new Ray(this.transform.position, transform.forward);
        Ray leftRay = new Ray(this.transform.position, Quaternion.AngleAxis(-raycastAngle, transform.up) * transform.forward);
        Ray rightRay = new Ray(this.transform.position, Quaternion.AngleAxis(raycastAngle, transform.up) * transform.forward);


        Vector3 direction = (obstacle.position - this.transform.position).normalized;


        //forward ray
        if (Physics.Raycast(centreRay, out hit, raycastLength))
        {
            if (hit.transform != transform)
            {
                //direction += hit.normal * 1;
                Debug.DrawRay(centreRay.origin, centreRay.direction * raycastLength, Color.red);
            }
        }

        //left ray
        if (Physics.Raycast(leftRay, out hit, raycastLength))
        {
            if (hit.transform != transform)
            {
                //direction += hit.normal * 1;
                Debug.DrawRay(leftRay.origin, leftRay.direction * raycastLength, Color.blue);
            }
        }

        //right ray
        if (Physics.Raycast(rightRay, out hit, raycastLength))
        {
            if (hit.transform != transform)
            {
                //direction += hit.normal * 1;
                Debug.DrawRay(rightRay.origin, rightRay.direction * raycastLength, Color.yellow);
            }
        }




        //Quaternion rot = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
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
            rb.velocity = velRotation * rb.velocity;
        }
        m_OldRotation = transform.eulerAngles.y;
    }

    void UpdateWheelMotions()
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

