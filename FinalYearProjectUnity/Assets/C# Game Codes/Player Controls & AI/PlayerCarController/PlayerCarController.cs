using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCarController : MonoBehaviour {

    // Car Movement 
    private float verticalMovement;
    private float horizontalMovement;
    private float steeringAngle;
    private float maxSteerAngle = 30;
    private float maxSpeed;
    private float reverseSpeed;
    public float setMaxSpeed;
    public float setMaxReverseSpeed;
    public float actualSpeed { get { return rb.velocity.magnitude * 2.23693629f; } }
    public float motorTorquePower = 50;
    public float brakeTorquePower = 100;
    public float brakingPower = 50f;

    GameObject nitroParticle;

    // Braking Bools
    public bool carForward;
    public bool carReverse;
    public bool isCarMovingForward;
    private bool isHandBraking;
    private bool isBraking;

    //Nitrous system pressing E and out
    bool isNitrousOn = false;
    public float maxNitroSpeed; // maximum speed the car go in nitrous mode
    float newTorquePower = 1000;
    float oldTorquePower = 500;


    //Updating Nitrous Values 
    public float currentNitro; // will be the max nitro
    float maxNitro = 100;// how much nitrous do we have.

    // Car Mathematics (Vectors, Quarternion, Rigidbody, Transforms and WheelCollider)
    private Vector3 wheelPos;
    private Quaternion wheelRot;
    public Rigidbody rb;
    public Transform transformWheelFrontLeft, transformWheelFrontRight;
    public Transform transformWheelRearLeft, transformWheelRearRight;
    public WheelCollider wheelFrontLeft, wheelFrontRight;
    public WheelCollider wheelRearLeft, wheelRearRight;

    void Start()
    {
        OnPlayerStartSettings();   
    }

    // Update is called once per frame
    void Update()
    {
        SetInput();
        SteerCar();
        AccelerateCar();
        NewNitroSystem();
        HandBrakeCar();
        CarBraking();
        UpdateNitroValue();
        UpdateWheelMotions();
        //Debug.Log(currentNitro);
    }

    void OnPlayerStartSettings()
    {
        rb = GetComponent<Rigidbody>(); // get the component of this rigidbody (playercar)
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.9f, 0.2f); // centre of mass to keep car stable
        currentNitro = maxNitro; // current nitro is now equal to the max nitro (100 at the start)
        rb = GetComponent<Rigidbody>(); // get the rigidbody thats attached to the car..
    }

    void SetInput()
    {
        // initialize and set up inputs for the car... 
        // moving forwards and backwards using the WASD or Arrow Keys

        verticalMovement = Input.GetAxis("Vertical");
        horizontalMovement = Input.GetAxis("Horizontal");
    }

    void SteerCar()
    {
        // set the steering angle to the maximum steer angle times by the horizontal movement variable
        // Assign them to the left and right wheels.
        steeringAngle = maxSteerAngle * horizontalMovement;
        wheelFrontLeft.steerAngle = steeringAngle;
        wheelFrontRight.steerAngle = steeringAngle;
    }

    void AccelerateCar()
    {
        maxSpeed = setMaxSpeed;
        reverseSpeed = setMaxReverseSpeed;

        if (actualSpeed >= setMaxSpeed)
        {
            verticalMovement = 0;
        }

        // set the motor torque power to the wheel colliders motorTorque member variable times by the vertical movement
        // Assign them to the left and right wheels. or vice versa doesnt matter really.
        wheelFrontLeft.motorTorque = motorTorquePower * verticalMovement;
        wheelFrontRight.motorTorque = motorTorquePower * verticalMovement;
        wheelRearLeft.motorTorque = motorTorquePower * verticalMovement;
        wheelRearRight.motorTorque = motorTorquePower * verticalMovement;

        // if the car is moving forward the bool is set to true
        if (verticalMovement == 1)
        {
            isCarMovingForward = true;
        }

        // if the car is moving backwards and not forward then set it to false
        if (verticalMovement == -1)
        {
            isCarMovingForward = false;
            //verticalMovement = 0;
        }

    }

    void NewNitroSystem()
    {
        // pass the max speed assigned and set it in another variable.
        maxSpeed = setMaxSpeed;

        // if the 'E' key is PRESSED
        // and the nitrous has not been activated
        // turn the nitrous on and increase the torque power of the car.
        // with nitrous on the car has the ability to reach the max nitro speed
        // else if 'E' key is RELEASED then revert back to the normal torque power of the car (no nitrous)
        if (Input.GetKey(KeyCode.E) && !isNitrousOn)
        {
            isNitrousOn = true;
            motorTorquePower = newTorquePower;
            setMaxSpeed = maxNitroSpeed;
            //nitroParticle.SetActive(true);
            //particle.play
            //Debug.Log("Nitro activated");
        }
        else if(Input.GetKeyUp(KeyCode.E) && isNitrousOn)
        { 
            isNitrousOn = false;
            //nitroParticle.SetActive(false);
            motorTorquePower = oldTorquePower;
           //Debug.Log("nitrous deactivated");
        }

        // if the nitro value is equal to zero OR is equal to 5
        // dont boost...
        if (Input.GetKey(KeyCode.E) && isNitrousOn && (currentNitro <= 5))
        {
            isNitrousOn = false;
            motorTorquePower = oldTorquePower;
        }
    }

    void UpdateNitroRate(int downRate, int upRate)
    {
        // this function allows the ability to
        // either..
        // decrease the nitro value when pressed
        // increase the nitro value when released

        currentNitro -= downRate * Time.deltaTime;
        currentNitro += upRate * Time.deltaTime;
    }

    void DecreasedNitroValue()
    {
        UpdateNitroRate(0, 1);
        //Debug.Log("Refilling Nitro Value");
    }


    void UpdateNitroValue()
    {
        // This function updates the nitro value based on whether the E key is pressed or not
        // If the key is pressed, decrease the nitro value
        // If the key is released, wait 5 SECONDS then refill the nitro value back to 100

        if (Input.GetKey(KeyCode.E))
        {
            UpdateNitroRate(20, 0);
            CancelInvoke();
            //Debug.Log("Decreased Nitro Value");
        }
        else
        {
            //Debug.Log("Wait 5 seconds to refill nitrous");
            Invoke("DecreasedNitroValue", 5);
        }


        // Safety Checks
        // Making sure the values are set to 0 and 100
        // If these numbers are ever reached.
        if (currentNitro <= 0)
        {
            currentNitro = 0;
        }

        if (currentNitro > 100)
        {
            currentNitro = 100;
        }
    }

    void HandBrakeCar()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("BRAKING!!");
            isHandBraking = true;
        }
        else
        {
            isHandBraking = false;
        }

        if (isHandBraking)
        {
            wheelFrontLeft.brakeTorque = brakeTorquePower;
            wheelFrontRight.brakeTorque = brakeTorquePower;

            wheelRearLeft.brakeTorque = brakeTorquePower;
            wheelRearRight.brakeTorque = brakeTorquePower;

            wheelFrontLeft.motorTorque = 0;
            wheelFrontRight.motorTorque = 0;

            wheelRearLeft.motorTorque = 0;
            wheelRearRight.motorTorque = 0;
        }
        else if (!isHandBraking && (Input.GetButton("Vertical") == true))
        {
            wheelFrontLeft.brakeTorque = 0;
            wheelFrontRight.brakeTorque = 0;
            wheelRearLeft.brakeTorque = 0;
            wheelRearRight.brakeTorque = 0;
        }
    }

    void CarBraking()
    {
        float carReverseMovement = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.S))
        {
            isBraking = true;
        }
        else
        {
            isBraking = false;
        }

        if (isBraking)
        {
            wheelFrontLeft.brakeTorque = brakingPower;
            wheelFrontRight.brakeTorque = brakingPower;
            wheelRearLeft.brakeTorque = brakingPower;
            wheelRearRight.brakeTorque = brakingPower;
        }

        if (isBraking)
        {
            wheelFrontLeft.motorTorque = carReverseMovement * motorTorquePower;
            wheelFrontRight.motorTorque = carReverseMovement * motorTorquePower;
            wheelRearLeft.motorTorque = carReverseMovement * motorTorquePower;
            wheelRearRight.motorTorque = carReverseMovement * motorTorquePower;

            wheelFrontLeft.brakeTorque = 0;
            wheelFrontRight.brakeTorque = 0;
            wheelRearLeft.brakeTorque = 0;
            wheelRearRight.brakeTorque = 0;
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
