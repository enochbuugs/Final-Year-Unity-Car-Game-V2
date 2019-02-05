using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNitroBar : MonoBehaviour {

    public Image nitroBar;
    float maxNitro = 100;
    float currentNitro;
    float t;
    float rechargeNitroRate;
    float nitroTimer = 1;
    bool isNitroRecharging;



	// Use this for initialization
	void Start () {

        currentNitro = maxNitro;
	}
	
	// Update is called once per frame
	void Update () {

        SetupNitroBarFill();
	}

    void SetupNitroBarFill()
    {
        nitroBar.fillAmount = currentNitro / maxNitro;
    }

    void NitroRechargeRate(float chargeRate)
    {
        t = 0f;
        currentNitro += chargeRate;
    }

    void RechargeNitroBar()
    {
        t += Time.deltaTime;

        if (currentNitro <= maxNitro)
        {
            if (t >= nitroTimer)
            {
                NitroRechargeRate(1);
            }
        }
    }
}
