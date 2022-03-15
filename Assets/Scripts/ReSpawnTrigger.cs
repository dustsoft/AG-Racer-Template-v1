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
        //SceneManager.LoadScene("City Nights (Track 01) Scene");

        player.transform.position = respawnPoint.transform.position;
    }
}
