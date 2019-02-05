using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySpeedMPH : MonoBehaviour {

    PlayerCarController carControl;
    public Text speedText;
    public GameObject playerCar;

    // Use this for initialization
    void Start ()
    {
        carControl = playerCar.GetComponent<PlayerCarController>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        speedText.text = "Speed: " + (int)carControl.actualSpeed + " mph";
	}
}
