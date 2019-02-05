using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScoreDamager
{
    void ScoreReduction(float amount);

    float getScoreReduction { get; set; }
}
