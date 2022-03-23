using UnityEngine;
using System.Collections;

public class ShipCameraScript : MonoBehaviour {

	public Transform target;
	ThrusterScript controller;
	public float distance = 6.0f;
	public float height = 1.5f;
	public float damping = 5f;
	public float rotationDamping = 15f;
	public float cameraKeepUpSpeed = 15f;
	[Tooltip("Change in field of view with speed")]
	public bool useZoom = true;
	[Tooltip("A fluid like lag when the camera follows")]
	public bool SmoothCameraFollow = true;
	[Tooltip("Make sure to tag your target as \"Player\"")]
	public bool autoFindTarget;

	Vector3 lookDir;
	float smoothYRot;
	Transform lookObj;
	Vector3 forwardLook;
	Vector3 upLook;
	Vector3 targetForward;
	Vector3 targetUp;
	Rigidbody rb;
	float fov;
	void Start () {


		//Set variables based on target vehicle's properties
		if (target)
		{
			controller = target.gameObject.GetComponent<ThrusterScript> ();
			forwardLook = target.forward;
			upLook = target.up;
			rb = target.GetComponent<Rigidbody>();

			GameObject look = new GameObject ("camLook");
			lookObj = look.transform;
			fov = GetComponent<Camera> ().fieldOfView;
		}
	}
	void FixedUpdate () {
		if (autoFindTarget) {
			GameObject player_ = GameObject.FindGameObjectWithTag ("Player");
			if (!player_)
				return;
			
				target = player_.transform;
			controller = target.gameObject.GetComponent<ThrusterScript> ();
		}
		if (target != null)
		{
			targetForward = target.forward;

			targetUp = (!controller.frontGrounded && !controller.backGrounded) ? Vector3.up : target.up;

			if (controller.frontGrounded || controller.backGrounded)
			{
				upLook = Vector3.Lerp(upLook, targetUp, Time.deltaTime * rotationDamping);
			}
			else
			{

				upLook = Vector3.Lerp(upLook, (targetUp/2) + new Vector3 (0f,height,0f), Time.deltaTime * rotationDamping);
			}

			lookDir = Vector3.Slerp(lookDir, Vector3.forward, 0.1f);
			smoothYRot = Mathf.Lerp(smoothYRot, rb.angularVelocity.y, Time.deltaTime * rotationDamping);

			//Calculate rotation and position variables
			forwardLook = Vector3.Lerp(forwardLook, targetForward, Time.deltaTime * damping);
			lookObj.rotation = Quaternion.LookRotation(forwardLook, upLook);
			GetComponent<Camera> ().fieldOfView = useZoom ? fov + rb.velocity.magnitude * 0.1f : fov;

			lookObj.position = SmoothCameraFollow ? Vector3.Lerp(lookObj.position,target.position,Time.deltaTime * cameraKeepUpSpeed) : target.position;
			//lookObj.position = target.position;
			Vector3 lookDirActual = (lookDir - new Vector3(Mathf.Sin(smoothYRot),0f, Mathf.Cos(smoothYRot)) * Mathf.Abs(smoothYRot) * 0.2f).normalized;
			Vector3 forwardDir = lookObj.TransformDirection(lookDirActual);
			//we use the lookObj to supress the rough camera jitter from landing a jump and not give you nausea
			Vector3 localOffset = lookObj.TransformPoint(-lookDirActual * distance - lookDirActual * Mathf.Min(rb.velocity.magnitude * 0.05f, 2) + new Vector3(0,height,0));
			transform.position = localOffset;
			transform.rotation = Quaternion.LookRotation(forwardDir, lookObj.up);
		}
	}

}