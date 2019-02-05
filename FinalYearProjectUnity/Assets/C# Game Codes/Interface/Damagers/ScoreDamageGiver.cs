using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDamageGiver : MonoBehaviour, IScoreDamager{

    PlayerScoreManager psm;
    public GameObject playerCar;

    public float getScoreReduction
    {

        get
        {
            return psm.currentScore;
        }

        set
        {
            psm.currentScore = value;
        }
    }

    public void ScoreReduction(float amount)
    {
        psm.currentScore -= amount;
        Debug.Log("I have taken: " + amount);
    }



    void RetrievePlayerScore()
    {
        psm = playerCar.GetComponent<PlayerScoreManager>();
        psm.SetScore();
    }

    void UpdatePlayerScore()
    {
        psm = playerCar.GetComponent<PlayerScoreManager>();
        psm.DisplayScore();
    }

    void Start()
    {
        RetrievePlayerScore();
    }

    void Update()
    {
        UpdatePlayerScore();
    }
}
