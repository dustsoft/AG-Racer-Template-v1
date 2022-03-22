using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapCompleteTrigger : MonoBehaviour
{
    public GameObject LapCompleteTrig;
    public GameObject HalfLapTrig;


    public LapTimeManager LapTime;

    void OnTriggerEnter()
    {
        LapTimeManager.MinuteCount = 0;
        LapTimeManager.SecondCount = 0;
        LapTimeManager.MilliCount = 0;
        LapTimeManager.MilliCountX = 0;



        HalfLapTrig.SetActive(true);
        LapCompleteTrig.SetActive(false);


        if (LapTimeManager.MinuteCount < LapTimeManager.BestMinute)
        {
            LapTimeManager.BestMinute = LapTimeManager.MinuteCount;
            //call function to update the UI
            LapTime.UpdateBestTimeText(LapTimeManager.BestMinute);

        }


    }
}
