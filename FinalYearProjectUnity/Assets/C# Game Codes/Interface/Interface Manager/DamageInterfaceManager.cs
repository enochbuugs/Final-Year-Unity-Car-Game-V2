using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THE MAIN INTERFACE FOR OBJECTS THAT CAN DAMAGE
public interface IDamageable
{ 
    // 3 levels of damage dealt
    void DamageTaken(float amout);


    // getter and setter for the damage dealt..
    float GetDamage {get; set;}
}
