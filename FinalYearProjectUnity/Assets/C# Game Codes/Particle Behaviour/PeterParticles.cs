using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeterParticles : MonoBehaviour {

    public ParticleSystem particle;
    public IEnumerator currentCoroutine; // the initial start couroutine

    [Range(0, 100)]
    public float emissionRate; // store the emission rate of the particle here

    private float t; // to store Time.deltaTime
    public float emissionOutTime; // How much time before next time to activate the particle?
    ParticleSystem.EmissionModule emission; // variable to get access to the emission variables of the particle system

    // Use this for initialization
    void Start()
    {
        SetupParticleLoop();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEmissionRate();
    }

    void SetupParticleLoop()
    {
        // Get the particle component system and store it into the particle variable
        // Store the emission member variable assigned to particle into the emission variable
        // Initialize and set the emission rate of the particle (50 particles spawned every frame)
        // Call the IEnumerator function 'WaitToEmit' as the current couroutine and start it.
        // This will start the particle system when the game starts 

        particle = GetComponent<ParticleSystem>();
        emission = particle.emission;
        emissionRate = 50f;
        currentCoroutine = WaitToEmit(); // store the couroutine created into currentCouroutine variable
        StartCoroutine(currentCoroutine); // call the 'WaitToEmit' function couroutine at the start
    }

    void UpdateEmissionRate()
    {
        // Emit 50 particles overtime each frame on Update 
        emission.rateOverTime = emissionRate;
    }

    IEnumerator WaitToEmit()
    {
        // start playing the particle animation for 5 seconds
        yield return new WaitForSeconds(5);

        // while 50 particles are spawned overtime and is greater than 0
        // reverse play the particles, from finish back to start with -Time.deltaTime
        while (emissionRate > 0)
        {
            emissionRate -= Time.deltaTime * emissionOutTime;

            yield return 0;
        }

        // wait for 5 seconds again...
        yield return new WaitForSeconds(5);

        // set the emission rate back to 50
        emissionRate = 50;

        // Stop this 'WaitToEmit' IEnumurator function to avoid infinite looping
        // Then start it again.. with StartCouroutine method
        StopCoroutine(currentCoroutine);
        currentCoroutine = WaitToEmit();
        StartCoroutine(currentCoroutine);
    }
}
