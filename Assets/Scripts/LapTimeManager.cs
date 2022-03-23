using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LapTimeManager : MonoBehaviour
{
    public static int MinuteCount = 00;
    public static int SecondCount = 00;
    public static float MilliCount = 0f;
    public static float MilliCountX = 0f;
    public static string MilliDisplay;
    public static string MilliDisplayX;

    public GameObject MinuteBox;
    public GameObject SecondBox;
    public GameObject MilliBox;
    public GameObject MilliXBox;

    public static float RawTime;


    void Update()
    {
        MilliCount += Time.deltaTime * 10;
        RawTime += Time.deltaTime;
        MilliDisplay = MilliCount.ToString("F0");
        MinuteBox.GetComponent<Text>().text = "" + MilliDisplay;

        if (MilliCount >= 10)
        {
            MilliCount -= 10;
            SecondCount += 1;
        }

        //I'm not sure what this does, found this code on YouTube
        MilliDisplay = Mathf.Floor(MilliCount).ToString("F0");
        MilliBox.GetComponent<Text>().text = "" + MilliDisplay;

        //X Display
        MilliCountX += Time.deltaTime * 100;
        MilliDisplayX = MilliCountX.ToString("F0");

        if (MilliCountX >= 10)
        {
            MilliCountX -= 10;
        }

        MilliDisplayX = Mathf.Floor(MilliCountX).ToString("F0");
        MilliXBox.GetComponent<Text>().text = "" + MilliDisplayX;
        //End X Display


        if (SecondCount <= 9)
        {
            SecondBox.GetComponent<Text>().text = "0" + SecondCount + "''";
        }
        else
        {
            SecondBox.GetComponent<Text>().text = "" + SecondCount + "''";
        }

        if (SecondCount >= 60)
        {
            SecondCount = 0;
            MinuteCount += 1;
        }

        if (MinuteCount <= 9)
        {
            MinuteBox.GetComponent<Text>().text = "0" + MinuteCount + "'";
        }
        else
        {
            MinuteBox.GetComponent<Text>().text = "" + MinuteCount + "'";
        }
    }


}
