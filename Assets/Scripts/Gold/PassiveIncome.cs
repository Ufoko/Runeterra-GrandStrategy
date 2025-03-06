using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveIncome : MonoBehaviour
{
    public float timerTime = 5f;
    public float Timer = 5f;
    void Update()
    {
        if (Player.local) {            
            Timer -= Time.deltaTime;
            if (Timer <= 0) {
                Player.local.gold += 2;
                Player.general.gold += 2;
                Timer = timerTime;
            }
        }
    }
}
