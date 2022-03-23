using UnityEngine;
using System.Collections;

public class ThrusterScript : MonoBehaviour {
	public float thrusterStrength = 2500f;
	public float thrusterDistance = 1.6f;
	public float thrusterDamping = 2000f;
	public Transform[] FrontThrusters;
	public Transform[] RearThrusters;
	[HideInInspector]
	public bool frontGrounded;
	[HideInInspector]
	public bool backGrounded;

	// compare raycast hit distance of last frame
	float lastHitDistF;
	float lastHitDistR;
	Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody> ();

	}

	// Update is called once per frame
	void FixedUpdate () {

		foreach (Transform Fthruster in FrontThrusters) {
			RaycastHit hit;

			if (Physics.Raycast (Fthruster.position, Fthruster.up * -1, out hit, thrusterDistance)) 
				frontGrounded = true;
			else
				frontGrounded = false;

			//keep ship at a midpoint. (when grounded) when it gets too low add strong force to raise it up
			// when it gets too high (off ground) add strong force to push it down
			// these forces all push the ship to its midpoint (AKA thruster height)

			//velocity of the 'spring'
			Vector3 velocity = rb.GetRelativePointVelocity(Fthruster.localPosition);

			lastHitDistF = hit.distance;
			if (frontGrounded) {
				float contactDepth = (thrusterDistance - hit.distance)/thrusterDistance;
				float springForce = thrusterStrength * contactDepth;

				float compressSpeed = contactDepth - lastHitDistF;
				float damper = thrusterDamping * Mathf.Abs(compressSpeed);
				Vector3 newForce = transform.up * ((springForce - damper) + Mathf.Abs(velocity.y));
				newForce *= rb.mass * Time.deltaTime;
				rb.AddForceAtPosition (newForce, Fthruster.position);

				lastHitDistF = contactDepth;
			} else {
				float intensity = 1 + rb.velocity.magnitude * 0.01f;
				rb.AddForceAtPosition (-transform.up * thrusterStrength * intensity, Fthruster.position);
			}

		}
		//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		foreach (Transform Rthruster in RearThrusters) {
			RaycastHit hit;

			if (Physics.Raycast (Rthruster.position, Rthruster.up * -1, out hit, thrusterDistance)) 
				backGrounded = true;
			else
				backGrounded = false;


			Vector3 velocity = rb.GetRelativePointVelocity(Rthruster.localPosition);
		//	downForce = transform.up * ((thrusterStrength * distPercent) * (Mathf.Abs(velocity.y) >= 20 && backGrounded ? Mathf.Abs(1+velocity.y/100f) : 1f)); //vector3
		//		downForce *= rb.mass * Time.deltaTime;

			lastHitDistR = hit.distance;

			if (backGrounded)
			{
				float contactDepth = (thrusterDistance - hit.distance)/thrusterDistance;
				float springForce = thrusterStrength * contactDepth;

				float compressSpeed = contactDepth - lastHitDistR;
				float damper = thrusterDamping * Mathf.Abs(compressSpeed);
				Vector3 newForce = transform.up * ((springForce - damper) + Mathf.Abs(velocity.y));
				newForce *= rb.mass * Time.deltaTime;
				rb.AddForceAtPosition (newForce, Rthruster.position);

				lastHitDistR = contactDepth;
			}
			else {
				float intensity = 1 + rb.velocity.magnitude * 0.01f;
				rb.AddForceAtPosition (-transform.up * thrusterStrength * intensity, Rthruster.position);
			}

		}
	}

	void OnDrawGizmos()
	{

		//this function serves no purpose in terms of gamePlay, but does
		//in terms of testing.
		//It makes those Rays visible by drawing rays and lines and spheres. All color coded.
		RaycastHit hit;

			foreach (Transform Rthruster in RearThrusters) {
			if (Physics.Raycast (Rthruster.position, Rthruster.up * -1, out hit, thrusterDistance)) {
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere (hit.point, 0.1f);
				Gizmos.DrawLine (Rthruster.position, hit.point);
			}
			else
			{//
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (Rthruster.position - transform.up * thrusterDistance, 0.1f);
				Gizmos.DrawLine (Rthruster.position, Rthruster.position - transform.up * thrusterDistance);
			}//
		}
		foreach (Transform Fthruster in FrontThrusters) {
			if (Physics.Raycast (Fthruster.position, Fthruster.up * -1, out hit, thrusterDistance)) {
				Gizmos.color = Color.red;
				Gizmos.DrawSphere (hit.point, 0.1f);
				Gizmos.DrawLine (Fthruster.position, hit.point);
			}
			else
			{
				Gizmos.color = Color.green;
				Gizmos.DrawSphere (Fthruster.position - transform.up * thrusterDistance, 0.1f);
				Gizmos.DrawLine (Fthruster.position, Fthruster.position - transform.up * thrusterDistance);
			}
		}
	}

	[ContextMenu("make thrusters")]
	//you can make more thrusters, but this is bare bones basic
	void MakeThrusters()
	{

		FrontThrusters = new Transform[2];
		RearThrusters = new Transform[2];

		GameObject fl = new GameObject ("ThrusterFL");
		fl.transform.SetParent (transform);
		fl.transform.localPosition = new Vector3 (-1.5f, 0f, 2.2f);
		FrontThrusters [0] = fl.transform;

		GameObject fr = new GameObject ("ThrusterFR");
		fr.transform.SetParent (transform);
		fr.transform.localPosition = new Vector3 (1.5f, 0f, 2.2f);
		FrontThrusters [1] = fr.transform;

		GameObject rl = new GameObject ("ThrusterRL");
		rl.transform.SetParent (transform);
		rl.transform.localPosition = new Vector3 (-1.5f, 0f, -2.2f);
		RearThrusters [0] = rl.transform;

		GameObject rr = new GameObject ("ThrusterRR");
		rr.transform.SetParent (transform);
		rr.transform.localPosition = new Vector3 (1.5f, 0f, -2.2f);
		RearThrusters [1] = rr.transform;

		Debug.Log ("Ding! 2 sets of thrusters coming right up!");

	}
		
}
