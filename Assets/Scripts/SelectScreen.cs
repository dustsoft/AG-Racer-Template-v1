using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectScreen : MonoBehaviour
{
    public void LoadCityNights()
    {
        LapTimeManager.MinuteCount = 0;
        LapTimeManager.SecondCount = 0;
        LapTimeManager.MilliCount = 0;
        LapTimeManager.MilliCountX = 0;

        LapTimeManager.RawTime = 0;

        SceneManager.LoadScene("City Nights (Track 01) Scene");
    }

    public void LoadStellarRoads()
    {
        LapTimeManager.MinuteCount = 0;
        LapTimeManager.SecondCount = 0;
        LapTimeManager.MilliCount = 0;
        LapTimeManager.MilliCountX = 0;

        LapTimeManager.RawTime = 0;

        SceneManager.LoadScene("Stellar Roads (Track 02) Scene");
    }
}
