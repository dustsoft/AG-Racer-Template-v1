using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectScreen : MonoBehaviour
{
    public void LoadCityNights()
    {
        SceneManager.LoadScene("City Nights (Track 01) Scene");
    }

    public void LoadStellarRoads()
    {
        SceneManager.LoadScene("Stellar Roads (Track 02) Scene");
    }
}
