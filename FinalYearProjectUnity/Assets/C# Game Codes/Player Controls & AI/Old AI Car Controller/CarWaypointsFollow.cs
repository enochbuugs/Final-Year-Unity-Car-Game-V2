using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWaypointsFollow : MonoBehaviour {

    //public GameObject carAI;
    //public Transform target;

    public Transform target;
    public AICarControllerOld aiCarControl;

    public float speed;
    public float accuracy;
    public float rotSpeed;

    int currentWP = 0; // what is the current waypoint

    void Start()
    {
        aiCarControl = GetComponent<AICarControllerOld>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToWaypoints();
    }

    void MoveToWaypoints()
    {

        // Get the waypoints transform position within x and z
        // Remember to keep the transform.position.y as stated to avoid the car having wierd behaviour... Must be in line with looking at the waypoint node..
        Vector3 lookAtWaypoint = new Vector3(target.position.x, transform.position.y, target.position.z);


        Vector3 direction = lookAtWaypoint - transform.position; // we want to get the direction between the waypoint and the car... 



        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);


        aiCarControl.MoveAICar();
        aiCarControl.UpdateWheelMotions();

        //if the direction to the waypoint is smaller than 1
        // or we are close enough to the waypoint..
        // move to the next waypoint
        //if (direction.magnitude < accuracy)
        //{
        //    currentWP++;
        //}

        // if the current waypoint number is equal to the waypoints maximum length 
        //if (currentWP >= circuit.Waypoints.Length)
        //{
        //    currentWP = 0;
        //}

        // call our functions in the ai controller base class

    }
}
