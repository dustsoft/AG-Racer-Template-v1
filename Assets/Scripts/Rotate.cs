using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float rotSpeed = 5f;

    void Update()
    {
        transform.Rotate(rotation * rotSpeed * Time.deltaTime);
    }
}
