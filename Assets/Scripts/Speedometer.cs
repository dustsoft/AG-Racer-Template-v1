using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    public Rigidbody target;
    public float maxSpeed = 0.0f; // The maximum speed of the target ** IN KM/H **

    [Header("UI")]
    public Text speedLabel; //label that displays the speed;

    private float speed = 0.0f;

    private void Update()
    {
        speed = target.velocity.magnitude * 3.6f * 1.09f; // tweak speed here second number

        if (speedLabel != null)
            speedLabel.text = ((int)speed) + "";
    }


}
