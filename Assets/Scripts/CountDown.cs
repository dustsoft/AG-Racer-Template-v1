using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    public GameObject countDown;
    public GameObject Laptimer;
    public GameObject GO;


    void Start()
    {

        StartCoroutine(CountStart());
    }

    IEnumerator CountStart ()
    {
        yield return new WaitForSeconds(0.5f);
        countDown.GetComponent<Text> ().text = "3";

        countDown.SetActive(true);
        yield return new WaitForSeconds(1);
        countDown.SetActive(false);
        countDown.GetComponent<Text>().text = "2";

        countDown.SetActive(true);
        yield return new WaitForSeconds(1);
        countDown.SetActive(false);
        countDown.GetComponent<Text>().text = "1";

        countDown.SetActive(true);
        yield return new WaitForSeconds(1);
        countDown.SetActive(false);
        countDown.GetComponent<Text>().text = "GO!";
        countDown.SetActive(true);

        PlayerPrefs.DeleteAll();

        //Start Timer
        Laptimer.SetActive (true); // set's laptimer UI to ACTIVE!

        GO.SetActive (true); //set's player ship object to ACTIVE!

        yield return new WaitForSeconds(1);
        countDown.SetActive(true);

    }
}
