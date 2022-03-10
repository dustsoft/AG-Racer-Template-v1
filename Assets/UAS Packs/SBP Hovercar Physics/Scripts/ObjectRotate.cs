using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotate : MonoBehaviour {

	public Vector3 rotateAxes;
	public GameObject[] ignoreCollisions;
	// Update is called once per frame
	void Update () {
		GetComponent<Rigidbody> ().AddTorque (rotateAxes * 5000f* GetComponent<Rigidbody>().mass);
		foreach (GameObject go in ignoreCollisions) {
		
			foreach (Collider col in GetComponentsInChildren<Collider>()) {
				
					Physics.IgnoreCollision (col, go.GetComponent<Collider>());

			
			}
		
		}
	}
}
