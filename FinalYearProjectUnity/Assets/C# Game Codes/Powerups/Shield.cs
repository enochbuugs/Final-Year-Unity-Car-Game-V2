using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    public GameObject player;
    PlayerHealthBar phb;


    // FULLY IMPLEMENTED SHIELD WORKING

    public float GetDamage
    {
        get
        {
            return phb.currentHealth;
        }

        set
        {
            phb.currentHealth = value;
        }
    }

    public void DamageTaken(float amount)
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start() {

        phb = player.GetComponent<PlayerHealthBar>();
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject == player))
        {
            phb = player.GetComponent<PlayerHealthBar>();
            phb.currentHealth = phb.maxHealth;
            phb.StartCoroutine(phb.Shield());
            Destroy(this.gameObject);
        }
    }

}
