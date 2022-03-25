using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadLapTime : MonoBehaviour
{
    public int MinCount;
    public int SecCount;
    public float MilliCount;
    public float MilliXCount;

    public GameObject MinDisplay;
    public GameObject SecDisplay;
    public GameObject MilliDisplay;
    public GameObject MilliXDisplay;


    void Start()
    {
        MinCount = PlayerPrefs.GetInt("MinSave");
        SecCount = PlayerPrefs.GetInt("SecSave");
        MilliCount = PlayerPrefs.GetFloat("MilliSave");
        MilliXCount = PlayerPrefs.GetFloat("MilliXSave");

        MinDisplay.GetComponent<Text>().text = "0" + MinCount + "'";
        SecDisplay.GetComponent<Text>().text = "0" + SecCount + "''";
        MilliDisplay.GetComponent<Text>().text = "" + MilliCount.ToString("F0");
        MilliXDisplay.GetComponent<Text>().text = "" + MilliXCount.ToString("F0");

    }
}
