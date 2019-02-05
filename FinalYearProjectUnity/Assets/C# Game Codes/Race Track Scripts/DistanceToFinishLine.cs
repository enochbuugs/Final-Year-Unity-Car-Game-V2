using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceToFinishLine : MonoBehaviour
{
    private PlayerScoreManager psm;
    public Text progressionText;

    public GameObject currentWayPoint;
    public GameObject playerCarBumper;
    public GameObject[] waypoints;
    public GameObject[] splines;
    public float[] splineDistances;
    
    public float distanceToWaypoint;
    private float lengthOfTrack;
    public float completed = 0; // how much have you completed of the track
    private float raycastLength = 20f;
    private int currentWaypointIndex = 0;
    public bool completedFinish = false;

    private float trackLength = 0f;
    private float splineNodesCompletionLength = 0;
    

    // Use this for initialization
    void Start()
    {
        CalculateSplineTrackDistance();
        GetLengthOfTrack();
    }

    // Update is called once per frame
    void Update()
    {
        CheckRaceCompletion();
    }

    void CalculateSplineTrackDistance()
    {
        splineDistances = new float[splines.Length];

        for (int i = 0; i < splines.Length - 1; i++)
        {
            splineDistances[i] = (splines[i + 1].transform.position - splines[i].transform.position).magnitude;
        }

        foreach (float length in splineDistances)
        {
            trackLength += length;
        }

    }

    void GetLengthOfTrack()
    {
        //previous checkpoint set to null
        // length of track set to 0 for check testing.
        GameObject previousWaypoint = null;
        lengthOfTrack = 0;

        // loop through the waypoints in the track and get the distance of the whole track
        foreach (GameObject WP in waypoints)
        {
            Debug.Log(WP);
            if (previousWaypoint != null)
            {
                lengthOfTrack += Vector3.Distance(previousWaypoint.transform.position, WP.transform.position);
            }

            previousWaypoint = WP;
        }

        //initialize the currentwaypoint to the array index of the last item in the array
        currentWayPoint = waypoints[waypoints.Length - 1];
    }

    void CheckRaceCompletion()
    {
        if (!completedFinish)
        {
            DistanceToWaypointNodes();
            RaycastToFinish();
        }
        psm = GetComponent<PlayerScoreManager>();

        if (psm.hasTriggeredStartLine)
        {
            DisplayProgressionUI();
        }
    }

    void RaycastToFinish()
    {
        RaycastHit rayhit;
        Ray newRay = new Ray(playerCarBumper.transform.position, transform.forward);

        if (Physics.Raycast(newRay, out rayhit, raycastLength))
        {
            if (rayhit.collider.gameObject.name == "StartLine")
            {
                distanceToWaypoint = Vector3.Distance(playerCarBumper.transform.position, rayhit.point);
                completed = 100 - (100 * distanceToWaypoint / lengthOfTrack);
                completed = Mathf.Clamp(completed, 0, 100);
            }
            Debug.DrawLine(playerCarBumper.transform.position, rayhit.point);
        }
        else
            Debug.DrawRay(newRay.origin, newRay.direction * raycastLength, Color.red);



        if (Physics.Raycast(newRay , out rayhit, raycastLength))
        {
            if (rayhit.collider.gameObject.name == "FinishLine")
            {
                distanceToWaypoint = Vector3.Distance(playerCarBumper.transform.position, rayhit.point);
                completed = 100 - (100 * distanceToWaypoint / lengthOfTrack);
                completed = Mathf.Clamp(completed, 0, 100);
            }
            Debug.DrawLine(playerCarBumper.transform.position, rayhit.point);
        }
        else
            Debug.DrawRay(newRay.origin, newRay.direction  * raycastLength, Color.red);
    }

    void DistanceToWaypointNodes()
    {
        //gets the distance between the first waypoint and the player
        //distanceToWaypoint = Vector3.Distance(playerCarBumper.transform.position, currentWayPoint.transform.position);
        //print(currentWaypointIndex);
        RaycastHit rayhit;
        Ray newRay = new Ray(playerCarBumper.transform.position, transform.forward);

        if (Physics.Raycast(newRay, out rayhit, raycastLength))
        {
            if (rayhit.collider.gameObject == splines[currentWaypointIndex + 1])
            {
                distanceToWaypoint = Vector3.Distance(playerCarBumper.transform.position, rayhit.point);
            }
        }
        else
        {
            distanceToWaypoint = Vector3.Distance(playerCarBumper.transform.position, splines[currentWaypointIndex + 1].transform.position);
        }

        float distanceTravelled = (splineDistances[currentWaypointIndex] - distanceToWaypoint) + splineNodesCompletionLength;
        completed = (100 * distanceTravelled / trackLength);

    }

    void DisplayProgressionUI()
    {
        progressionText.text = "Progress: " + (int)completed + " %";
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        TriggerWaypoints(other);
    }

    void TriggerWaypoints(Collider other)
    {
        // if the player touches the start line 
        // they can start incrementing their score by driving forward
        // start line bool is responsible for incrementing score till the finish line reached...

        if (other.gameObject.name == "StartLine")
        {
            Debug.Log("Triggered " +other.gameObject.name);
            other.gameObject.GetComponent<Collider>().enabled = false; // disable collider
            psm = GetComponent<PlayerScoreManager>();
            psm.hasTriggeredStartLine = true; // set the bool to be true to start incrementing score
        }

        if (other.gameObject.tag == "SplineWaypoint")
        {
            splineNodesCompletionLength += splineDistances[currentWaypointIndex];
            other.gameObject.SetActive(false); // whatver spline you hit last set that to false
            currentWaypointIndex++;
        }

        // if the player touches the finish line
        // disable the start line bool which stops incrementing score
        // set the race completion bool to true
        // as long as the race has been 100% complete 

        if (other.gameObject.name == "FinishLine")
        {
            psm = GetComponent<PlayerScoreManager>();
            Debug.Log("You win!");
            other.gameObject.GetComponent<Collider>().enabled = false;
            completedFinish = true; // the race is now finished
            psm.hasTriggeredStartLine = false; // we are no longer incrementing score from when we hit the start line
            completed = 100f;
            DisplayProgressionUI();
        }
    }
}