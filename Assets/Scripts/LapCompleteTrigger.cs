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

    public float RawTime;



    void OnTriggerEnter()
    {
        RawTime = PlayerPrefs.GetFloat("RawTime");
        if (LapTimeManager.RawTime <= RawTime)
        {
            if (LapTimeManager.SecondCount <= 9)
            {
                SecondDisplay.GetComponent<Text>().text = "" + LapTimeManager.SecondCount + "''";
            }
            else
            {
                SecondDisplay.GetComponent<Text>().text = "" + LapTimeManager.SecondCount + "''";
            }

            if (LapTimeManager.MinuteCount <= 9)
            {
                MinuteDisplay.GetComponent<Text>().text = "0" + LapTimeManager.MinuteCount + "'";
            }
            else
            {
                MinuteDisplay.GetComponent<Text>().text = "0" + LapTimeManager.MinuteCount + "'";
            }

            if (LapTimeManager.MilliCount <= 9)
            {
                MilliDisplay.GetComponent<Text>().text = "" + LapTimeManager.MilliCount.ToString("F0") + "";
            }
            else
            {
                MilliDisplay.GetComponent<Text>().text = "" + LapTimeManager.MilliDisplay + "";
            }

            if (LapTimeManager.MilliCountX <= 9)
            {
                MilliXDisplay.GetComponent<Text>().text = "" + LapTimeManager.MilliCountX.ToString("F0") + "";
            }
            else
            {
                MilliXDisplay.GetComponent<Text>().text = "" + LapTimeManager.MilliCountX + "";
            }
        }

        PlayerPrefs.SetInt("MinSave", LapTimeManager.MinuteCount);
        PlayerPrefs.SetInt("SecSave", LapTimeManager.SecondCount);
        PlayerPrefs.SetFloat("MilliSave", LapTimeManager.MilliCount);
        PlayerPrefs.SetFloat("MilliXSave", LapTimeManager.MilliCountX);
        PlayerPrefs.SetFloat("RawTime", LapTimeManager.RawTime);

        LapTimeManager.MinuteCount = 0;
        LapTimeManager.SecondCount = 0;
        LapTimeManager.MilliCount = 0;
        LapTimeManager.MilliCountX = 0;

        LapTimeManager.RawTime = 0;

        HalfLapTrig.SetActive(true);
        LapCompleteTrig.SetActive(false);

    }
}
