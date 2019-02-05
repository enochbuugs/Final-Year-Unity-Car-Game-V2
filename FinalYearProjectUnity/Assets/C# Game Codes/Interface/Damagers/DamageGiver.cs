using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageGiver : MonoBehaviour, IDamageable {

    PlayerHealthBar pb;
    public GameObject playerCar;

    public float GetDamage
    {
        get
        {
            return pb.currentHealth;
        }

        set
        {
            pb.currentHealth = value;
        }
    }

    public void DamageTaken(float amount)
    {
        pb.currentHealth -= amount;
        Debug.Log("I have taken: " + amount);
    }



    void RetrievePlayerCarHealth()
    {
        pb = playerCar.GetComponent<PlayerHealthBar>();
        pb.currentHealth = pb.maxHealth;
    }
    
    void UpdatePlayerCarHealth()
    {
        pb = playerCar.GetComponent<PlayerHealthBar>();
        pb.healthBar.fillAmount = pb.currentHealth / pb.maxHealth;
    }

    void Start()
    {
        RetrievePlayerCarHealth();
    }

    void Update()
    {
        UpdatePlayerCarHealth();
    }
}
