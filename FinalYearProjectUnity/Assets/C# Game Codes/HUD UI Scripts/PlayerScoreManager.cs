using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreManager : MonoBehaviour {

    // script references
    PlayerCarController pc;
    PlayerHealthBar phb;

    // score variables
    public Text scoreText;
    private float score = 0;
    public float currentScore;
    public float pausedScore;
    public float finalScore;
    public float maxScore = 1000f;

    // score penalty timers
    public float easyPenaltyTimer = 5f;
    public float meduimPenaltyTimer = 10f;
    public float hardPenaltyTimer = 15f;

    // booleans
    public bool isScorePaused;
    public bool hasTriggeredStartLine;

    DistanceToFinishLine dtfl;

    // Use this for initialization
    void Start()
    {
        SetScore();
    }

    // Update is called once per frame
    void Update()
    {
        ClampScore();
        DisplayScore();
        IncrementScore(5);
        GetFinalScore();
    }

    public void SetScore()
    {
        currentScore = score;
    }

    void ClampScore()
    {
        currentScore = Mathf.Clamp(currentScore, 0, Mathf.Infinity);
        pausedScore = Mathf.Clamp(pausedScore, 0, Mathf.Infinity);
    }

    public void DisplayScore()
    {
        if (isScorePaused)
        {
            scoreText.text = "Score: " + (int)pausedScore;
        }
        else
        {
            scoreText.text = "Score: " + (int)currentScore;
        }
    }

    public void IncrementScore(float increaseRate)
    {
        pc = GetComponent<PlayerCarController>();

        dtfl = GetComponent<DistanceToFinishLine>();

        // if the car is moving forward
        // the race has not been completed
        // and the start line has not been triggered
        // do not increment the score..
        // avoids cheating

        if (pc.isCarMovingForward && !dtfl.completedFinish && !hasTriggeredStartLine)
        {
            hasTriggeredStartLine = false;
            currentScore += 0 * Time.deltaTime;
            dtfl.completedFinish = false;
        }

        // if the car is moving, the race is not completed and has detected that the start line has been triggered
        // increment the score..
        
        if (pc.isCarMovingForward && !dtfl.completedFinish && hasTriggeredStartLine)
        {
            hasTriggeredStartLine = true;
            currentScore += increaseRate * Time.deltaTime;
            dtfl.completedFinish = false;
        }
    }

    void GetFinalScore()
    {
        pc = GetComponent<PlayerCarController>();

        dtfl = GetComponent<DistanceToFinishLine>();

        if (pc.isCarMovingForward && dtfl.completedFinish)
        {
            dtfl.completedFinish = true;
            finalScore = currentScore;
        }
    }
}
