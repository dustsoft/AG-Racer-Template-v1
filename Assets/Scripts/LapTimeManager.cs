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

    

    private Text MinuteBox_Text;
    private Text MilliBox_Text;

    public Text BestTimeTextBox;

    public static float BestMinute = 999999f;

    // Update is called once per frame

    //EDITED: unity calls this at the start of a scene
    private void Start()
    {
        MinuteBox_Text = MinuteBox.GetComponent<Text>();
        MilliBox_Text = MilliBox.GetComponent<Text>();
    }

    void Update()
    {
        MilliCount += Time.deltaTime * 10;
        MilliDisplay = MilliCount.ToString("F0");
        MinuteBox_Text.text = "" + MilliDisplay;

        if (MilliCount >= 10)
        {
            MilliCount -= 10;
            SecondCount += 1;
        }

        //I'm not sure what this does, found this code on YouTube
        MilliDisplay = Mathf.Floor(MilliCount).ToString("F0");
        MilliBox_Text.text = "" + MilliDisplay;

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
            MinuteBox_Text.text = "0" + MinuteCount + "'";
        }
        else
        {
            MinuteBox_Text.text = "" + MinuteCount + "'";
        }
    }

    public void UpdateBestTimeText(float Minute)
    {
        BestTimeTextBox.text = Minute.ToString() + "'00''00";
    }
}
