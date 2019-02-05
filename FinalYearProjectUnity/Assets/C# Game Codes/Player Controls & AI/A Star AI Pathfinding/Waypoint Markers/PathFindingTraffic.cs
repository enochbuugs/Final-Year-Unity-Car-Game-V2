using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingTraffic : MonoBehaviour {

    
    float speed = 5.0f;
    float accuracy = 1.0f;
    //float rotSpeed = 2.0f;

    public GameObject waypointManager;
    private GameObject[] wps;
    GameObject currentNode;
    int currentWP = 0;
    Graph g;
    bool sendToCubeBuilding;

    [Header("AI Avoidance Properties")]
    Transform obstacle;

    Transform target;
    private Vector3 lookAtTarget; // holds the info to look at the target we should follow...
    private Vector3 distance; // the distance between the player and target it has to follow..
    private Vector3 wheelPos;
    private Quaternion wheelRot;
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


    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.9f, 0.2f);

        wps = waypointManager.GetComponent<WPManager>().waypoints;
        g = waypointManager.GetComponent<WPManager>().graph;
        currentNode = wps[0];
        sendToCubeBuilding = true;
        GoToCubeBuilding();
    }

    public void GoToCubeBuilding()
    {
        g.AStar(currentNode, wps[1]);
        currentWP = 0;
    }
	
    public void GoCylinderBuilding()
    {
        g.AStar(currentNode, wps[6]);
        currentWP = 0;
    }
	// Update is called once per frame
	void Update ()
    {
        Debug.Log(currentSpeed);
        UpdateWheelMotions();
        //Avoidance();




        GoPathfinding();
	}

    void GoPathfinding()
    {
        // if the path length is zero or the current waypoint is equal to the path length
        // this means in the A* algo that you are at the end.
        if (g.getPathLength() == 0 || currentWP == g.getPathLength())
        {
            if (sendToCubeBuilding)
            {
                GoCylinderBuilding();
                sendToCubeBuilding = false;
            }
            else
            {
                GoToCubeBuilding();
                sendToCubeBuilding = true;
            }
            return;
        }

        currentNode = g.getPathPoint(currentWP);

        // if we get close enough to the current waypoint
        // move to the next one...
        if (Vector3.Distance(g.getPathPoint(currentWP).transform.position, transform.position) < accuracy)
        {
            currentWP++;
        }

        // if we didnt hit the end of the path then keep going...
        if (currentWP < g.getPathLength())
        {
            target = g.getPathPoint(currentWP).transform;

            Vector3 lookAtTheTarget = new Vector3(target.position.x, this.transform.position.y, target.position.z);

            Vector3 direction = lookAtTheTarget - transform.position;

            transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);

            // losing speed every update with these functions.. needs a hotfix
            //MoveCar();

            // hotfix for now with no wheel colliders
            transform.Translate(0, 0, speed * Time.deltaTime);

        }
    }

    void UpdateWheelMotions()
    {
        // This method updates all the wheel colliders and the wheel mesh in real time as they ride together.
        UpdateWheelMotion(wheelFrontLeft, transformWheelFrontLeft);
        UpdateWheelMotion(wheelFrontRight, transformWheelFrontRight);
        UpdateWheelMotion(wheelRearLeft, transformWheelRearLeft);
        UpdateWheelMotion(wheelRearRight, transformWheelRearRight);
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
}
