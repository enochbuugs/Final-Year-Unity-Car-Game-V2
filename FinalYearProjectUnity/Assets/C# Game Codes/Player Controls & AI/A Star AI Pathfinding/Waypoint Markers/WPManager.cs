using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a struct is created to group variables together under one name
// in a block of memory
// this struct defines the nodes and the edges of the pathfinding route of the nodes placed
[System.Serializable]
public struct Link
{
    public enum direction { UNI, BI }; // UNI  goes in one direction and BI means it can travel 3 different nodes.
    public GameObject node1;
    public GameObject node2;
    public direction dir;
}

public class WPManager : MonoBehaviour {

    public GameObject[] waypoints;
    public Link[] links;
    public Graph graph = new Graph();

	// Use this for initialization
	void Start ()
    {
		if (waypoints.Length > 0)
        {
            // each node must exist
            // loop through all waypoints
            // and add the node if their are waypoints
            // using the addnode method
            foreach (GameObject wp in waypoints)
            {
                graph.AddNode(wp);
            }

            // loop through all links to add links to the waypoints
            // from UNI direction to BI direction
            foreach (Link l in links)
            {
                graph.AddEdge(l.node1, l.node2);

                if (l.dir == Link.direction.BI)
                {
                    graph.AddEdge(l.node2, l.node1);
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        graph.debugDraw();
	}
}
