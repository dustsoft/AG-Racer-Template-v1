using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapCompleteTrigger : MonoBehaviour
{
    public GameObject LapCompleteTrig;
    public GameObject HalfLapTrig;

    public GameObject MinuteDisplay;
    public GameObject SecondDisplay;
    public GameObject MilliDisplay;
    public GameObject MilliXDisplay;

    void OnTriggerEnter()
    {
        if (LapTimeManager.SecondCount <= 99)
        {
            SecondDisplay.GetComponent<Text>().text = "" + LapTimeManager.SecondCount + "''";
        }
        else
        {
            SecondDisplay.GetComponent<Text>().text = "" + LapTimeManager.SecondCount + "''";
        }

        if (LapTimeManager.MinuteCount <= 99)
        {
            MinuteDisplay.GetComponent<Text>().text = "0" + LapTimeManager.MinuteCount + "'";
        }
        else
        {
            MinuteDisplay.GetComponent<Text>().text = "" + LapTimeManager.MinuteCount + "'";
        }



        if (LapTimeManager.MilliCount <= 9)
        {
            MilliDisplay.GetComponent<Text>().text = "" + LapTimeManager.MilliCount.ToString("F0") + "";
        }
        else
        {
            MilliDisplay.GetComponent<Text>().text = "" + LapTimeManager.MilliDisplay + "";
        }




        if (LapTimeManager.MilliCountX <= 99)
        {
            MilliXDisplay.GetComponent<Text>().text = "" + LapTimeManager.MilliCountX.ToString("F0") + "";
        }
        else
        {
            MilliXDisplay.GetComponent<Text>().text = "" + LapTimeManager.MilliCountX + "";
        }

        LapTimeManager.MinuteCount = 0;
        LapTimeManager.SecondCount = 0;
        LapTimeManager.MilliCount = 0;
        LapTimeManager.MilliCountX = 0;

        HalfLapTrig.SetActive(true);
        LapCompleteTrig.SetActive(false);

    }
}
