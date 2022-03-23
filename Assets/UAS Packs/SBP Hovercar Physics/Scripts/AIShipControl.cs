using UnityEngine;
using System.Collections;

public class AIShipControl : MonoBehaviour {
	[Tooltip("If so, mark the 1st waypoint as \"FirstWaypoint\"")]
	public bool autoFindFirstWaypoint;
	public WaypointNodeScript nextTarget;
	[HideInInspector]
	public WaypointNodeScript lastTarget;
	[Header ("Movement")]
	public float thrustTorque = 7000f;
	public float brakeTorque = 6000f;
	public float grip = 1400f;
	[Range(0,180)]
	public float rotateRate = 90f;
	public float Downforce = 300f;
	[Space(20)]
	public float respawnWait = 2f;

	[Header ("Rotation")]
	public Vector3 rotationCorrectionPower = new Vector3 (20,0,40);
	public GameObject shipChildBody;
	[Range(0f,90f)]
	public float bodyRoll = 25f;
	[Range (0f,1f)]
	public float bodyRollSeekSpeed = 0.1f;
	public float turnYawAngle;
	public float turnYawSeekSpeed;
	[Range(0f,90f)]
	//private stuff
	private float rotationVelocity;
	private float groundingAngleVelocity;
	[Header("Health and Energy")]
	public float health;
	public float damageResistance = 20f;
	public bool useHealth = true;

	float forwardDot;
	float sideDot;
	float upDot;
	float steer;
	public float throttle;
	float brake;
	bool braking;
	bool shipDestroyed;
	float respawnTimer = 0.0f;
	Rigidbody rb;
	Vector3 turnVec;
	Vector3 imp;
	Vector3 tempVEC;
	float actualTurn;
	float actualGrip;
	float slideSpeed;
	float currentSpeed;
	Vector3 flatVelo;

	void Start ()
	{
		health = 100f;
		rb = GetComponent<Rigidbody> ();
		if (autoFindFirstWaypoint) {
			nextTarget = GameObject.FindGameObjectWithTag ("FirstWaypoint").GetComponent<WaypointNodeScript> ();
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		
		forwardDot = Vector3.Dot (transform.forward, Vector3.up);
		sideDot = Vector3.Dot  (transform.right, Vector3.up);
		upDot = Vector3.Dot  (transform.up, Vector3.up);
		health = Mathf.Clamp (health, 0f, 100f);
		if (health > 0f) {
			Move ();
			GetSteer ();
			CheckForRespawn ();
		} else {
			StartCoroutine (resetAIShip ());
		}
	}

	IEnumerator resetAIShip()
	{
		GetComponent<ThrusterScript> ().enabled = false;
		yield return new WaitForSeconds (3);
		GetComponent<ThrusterScript> ().enabled = true;
		shipDestroyed = false;
		health = 100f;
	}

	public void GetSteer()
	{
		if ((transform.position - nextTarget.transform.position).sqrMagnitude <= Mathf.Pow (nextTarget.radius, 2)) {
		
			lastTarget = nextTarget;
			nextTarget = nextTarget.nextNode;
		
		}

		Vector3 steerVector;

		steerVector = transform.InverseTransformPoint (nextTarget.transform.position.x, nextTarget.transform.position.y, nextTarget.transform.position.z);

		steer = Mathf.Clamp ((steerVector.x / steerVector.magnitude), -1, 1);

		//the plane of steering needs to be in all 3 directions, because we can drive on walls and upside down and other crazy angles
		tempVEC = new Vector3 (rb.velocity.x, rb.velocity.y,rb.velocity.z);

		flatVelo = tempVEC;

		tempVEC = new Vector3 (Vector3.right.x, Vector3.up.y,Vector3.forward.z);

		slideSpeed = Vector3.Dot (transform.right, flatVelo);
		currentSpeed = flatVelo.magnitude;

		turnVec = Vector3.up * (((rotateRate*0.01f) * steer))  * rb.mass;
		actualGrip = Mathf.Lerp (100, grip, currentSpeed * 0.02f);
		imp = transform.right * (-slideSpeed * rb.mass * actualGrip);

		transform.Rotate (turnVec * Time.deltaTime);
		rb.AddForce (imp * Time.deltaTime);

		Vector3 newRot = shipChildBody.transform.localEulerAngles;
		newRot.z = Mathf.SmoothDampAngle (newRot.z, -bodyRoll * steer, ref rotationVelocity, bodyRollSeekSpeed);
		shipChildBody.transform.localEulerAngles = newRot;
	}
	public void Move()
	{

		if (GetComponent<ThrusterScript>().frontGrounded || GetComponent<ThrusterScript>().backGrounded) {
			rb.angularDrag = 1f;
			rb.drag = 1f + (braking ? 0.5f : 0f);
			throttle = braking ? 0.5f : 1f;

			throttle = Mathf.Clamp01 (throttle);
			Vector3 thrustForce = transform.forward * thrustTorque * throttle;
			thrustForce = thrustForce * Time.deltaTime * rb.mass;
			rb.AddForce (thrustForce);


		} else {
			throttle = 0f;
			rb.drag = 0.1f;
			rb.angularDrag = 1f;
		}

		rb.AddForce (-transform.up * Downforce * rb.velocity.magnitude);
		
	}



	void CorrectRotation()
	{
		float actualForwardDir = forwardDot;
		float actualSideDir = sideDot;
		float actualUpDir = upDot;

		RaycastHit hit;

		if (Physics.Raycast(transform.position,
			(-Vector3.up + rb.velocity).normalized, out hit, 250f))
		{
			if (Vector3.Dot(hit.normal, Vector3.up) >= 0.5f)
			{
				actualForwardDir = Vector3.Dot(transform.forward, hit.normal);
				actualSideDir = Vector3.Dot(transform.right, hit.normal);
				actualUpDir = Vector3.Dot(transform.up, hit.normal);
			}
		}

		rb.AddRelativeTorque
		(
			new Vector3
			(
				//vertical input
				actualForwardDir * (1 - Mathf.Abs(actualSideDir)) * rotationCorrectionPower.x - transform.InverseTransformDirection(rb.angularVelocity).x * Mathf.Pow(actualUpDir, 2) * 10,
				//horizontal input
				Vector3.Dot(transform.forward,Vector3.right) * Mathf.Abs(actualUpDir) * rotationCorrectionPower.y - transform.InverseTransformDirection(rb.angularVelocity).y * Mathf.Pow(actualUpDir, 2) * 10,
				//horizontal input
				-actualSideDir * (1 - Mathf.Abs(actualForwardDir)) * rotationCorrectionPower.z - transform.InverseTransformDirection(rb.angularVelocity).z * Mathf.Pow(actualUpDir, 2) * 10
			)
			, 
			ForceMode.Acceleration
		);

	}

	void OnCollisionEnter(Collision other)
	{

		foreach (ContactPoint p in other.contacts)
		{
			if (Vector3.Dot (p.normal, transform.up) < 0.5f && useHealth && !shipDestroyed)
			{
				if (health > 0) {
					ApplyDamage ((other.impulse / Time.fixedDeltaTime).magnitude /rb.mass / 10f);
				} else {
					shipDestroyed = true;

				}


			}

		}

	}

	void OnTriggerEnter(Collider other)
	{

		if (other.transform.tag == "brakeZone") {
		
			braking = true;
		}

	}

	void OnTriggerStay(Collider other)
	{
	
		if (other.tag == "BoostPad") {
			rb.AddForce (other.transform.forward * 500f,ForceMode.Acceleration);
		}

	}
	void OnTriggerExit(Collider other)
	{

		if (other.transform.tag == "brakeZone") {

			braking = false;
		}

	}

	bool respawning;
	void CheckForRespawn()
	{
		if (rb.velocity.magnitude < 5) {
		
			respawnTimer += Time.deltaTime;
			if (respawnTimer >= respawnWait && !respawning) {
			
				respawning = true;
				transform.position = lastTarget.transform.position;
				Vector3 rotation = transform.localEulerAngles;
				rotation.z = 0f;
				transform.localEulerAngles = rotation;
			}
				
		
		} else {
		
			respawning = false;
			respawnTimer = 0.0f;
		
		}

		//if i get too far away from the target
		if ((transform.position - nextTarget.transform.position).magnitude > 1000) {
		
			transform.position = nextTarget.transform.position;
			Vector3 rotation = transform.localEulerAngles;
			rotation.z = 0f;
			transform.localEulerAngles = rotation;


		}

	}

	void OnDrawGizmos()
	{

		Gizmos.color = Color.cyan;
		if (nextTarget)
		Gizmos.DrawLine (transform.position, nextTarget.transform.position);

	}
	public void ApplyDamage (float damage)
	{


		health -= damage/damageResistance;	

	}
	public void SpeedBoost(Vector3 direction)
	{
		rb.AddForce (transform.InverseTransformDirection(direction) * 3000f);
	}

}
