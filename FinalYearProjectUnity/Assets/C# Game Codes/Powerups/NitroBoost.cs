using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroBoost : MonoBehaviour {

    public GameObject player;
    private PlayerCarController pc;

	// Use this for initialization
	void Start ()
    {
        pc = player.GetComponent<PlayerCarController>();
	}

    void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject == player) && (pc.currentNitro == 100f))
        {
            other.GetComponentInParent<PlayerCarController>();
            AddNitroBoost(0);
            pc.currentNitro = 100f;
            Destroy(this.gameObject);
        }

        if ((other.gameObject == player) && (pc.currentNitro < 100f))
        {
            other.GetComponentInParent<PlayerCarController>();
            AddNitroBoost(10);
            Destroy(this.gameObject);
        }
    }

    void AddNitroBoost(float amount)
    {
        pc.currentNitro += amount;
    }
}
