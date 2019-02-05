using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour {

    public GameObject player;
    private PlayerHealthBar phb;

    void Start()
    {
        phb = player.GetComponent<PlayerHealthBar>();
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject == player) && (phb.currentHealth == 100f))
        {
            other.GetComponentInParent<PlayerHealthBar>();
            AddCarHealth(0f);
            phb.currentHealth = 100f;
            Destroy(this.gameObject);
        }

        if ((other.gameObject == player) && (phb.currentHealth < 100f))
        {
            other.GetComponentInParent<PlayerHealthBar>();
            Debug.Log("Taking Health Pack");
            AddCarHealth(5f);
            Destroy(this.gameObject);
        }
    }

    void AddCarHealth(float amount)
    {
        phb.currentHealth += amount;
    }
}
