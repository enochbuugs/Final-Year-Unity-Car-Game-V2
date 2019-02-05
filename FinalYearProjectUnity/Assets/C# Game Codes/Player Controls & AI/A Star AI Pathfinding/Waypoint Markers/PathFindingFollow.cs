using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingFollow : MonoBehaviour {

    Transform target;
    float speed = 5.0f;
    float accuracy = 1.0f;
    float rotSpeed = 2.0f;

    public GameObject waypointManager;
    private GameObject[] wps;
    GameObject currentNode;
    int currentWP = 0;
    Graph g;
    bool sendToHeliPad;


    // Use this for initialization
    void Start ()
    {
        wps = waypointManager.GetComponent<WPManager>().waypoints;
        g = waypointManager.GetComponent<WPManager>().graph;
        currentNode = wps[0];
        sendToHeliPad = true;
        GoToCubeBuilding();
        //GoToRuin();
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
        // if the path length is zero or the current waypoint is equal to the path length
        // this means in the A* algo that you are at the end.
        if (g.getPathLength() == 0 || currentWP == g.getPathLength())
        {
            if (sendToHeliPad)
            {
                GoCylinderBuilding();
                sendToHeliPad = false;
            }
            else
            {
                GoToCubeBuilding();
                sendToHeliPad = true;
            }

            return;
        }

        currentNode = g.getPathPoint(currentWP);

        // if we get close enough to the current waypoint
        // move to the next one...
        if(Vector3.Distance(g.getPathPoint(currentWP).transform.position, transform.position) < accuracy)
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

            transform.Translate(0, 0, speed * Time.deltaTime);

        }
	}
}
