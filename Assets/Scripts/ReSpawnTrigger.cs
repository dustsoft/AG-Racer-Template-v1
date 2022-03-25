using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReSpawnTrigger : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawnPoint;


    void OnTriggerEnter(Collider other)
    {

        SceneManager.LoadScene("City Nights (Track 01) Scene");

        LapTimeManager.MinuteCount = 0;
        LapTimeManager.SecondCount = 0;
        LapTimeManager.MilliCount = 0;
        LapTimeManager.MilliCountX = 0;
    }
}
