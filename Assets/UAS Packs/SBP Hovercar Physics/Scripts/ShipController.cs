using UnityEngine;
using System.Collections;




[RequireComponent(typeof(AudioSource))]
//[RequireComponent(typeof(ThrusterScript))]
[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour {

	public bool touchScreenControls;
	public Transform vehicleChildObj;
	[Header("Movement")]
	[Tooltip("how fast the ship will speed up (also determines top speed)")]
	public float accel = 6000f;
	public float brakePower = 2000f;
	public float rotateRate = 90f;
	[Tooltip("How much the ship will stay close to the ground. when high enough, you can drive on walls, or even upsidedown!")]
	public float Downforce = 500f;
	[Tooltip("Can the ship reverse?")]
	public bool canReverse = true;
	[Tooltip("reverse torque. also determines top reverse speed")]
	public float reversePower = 500f;
	public float grip = 1400f;
	public bool disableControls = false;

	[Header("Rotation")]
	[Range(0,90)]
	public float bodyRoll = 25f;
	[Range(0.05f,1f)]
	public float bodyRollSeekSpeed = 0.1f;
	public float CurrentSpeed;
	private float rotationVelocity;
	[Tooltip("strength of pitch, yaw, and roll when correcting rotation")]
	public Vector3 rotationCorrectionPower = new Vector3 (20,0,40);
	[Header("Audio")]
	[Range(0,5)]
	public float minEnginePitch = 0.5f;
	[Range(0,5)]
	public float maxEnginePitch = 2f;
	[Range(0,3)]
	public float maxFlightWindVolume = 1.0f;
	public AudioClip engineSound;
	public AudioClip flightWindSound;
	[Header("Boost and Energy")]
	[HideInInspector]
	public float energyLeft;
	[HideInInspector]
	public float healthLeft;
	public float boostTorque = 2500f;
	public float boostConsumptionRate = 35f;
	public float boostRefillRate = 20f;
	[Tooltip("Dividing factor in the collision impulse to dull down damage. Higher value = more strength")]
	public float strength = 30f;
	[Tooltip("your boost bar also counts as your health bar. you can boost yourself to death")]
	public bool boostIsHealth = false;
	[Tooltip("Make your ship vulnerable?")]
	public bool useHealth = true;
	[Tooltip("the ball of energy that appears around your ship when restoring energy")]
	public GameObject healingCapsule;
	[Tooltip("your boost automatically fills up")]
	public bool autoBoostRegeneration = false;
	public GameObject collisionParticle;
	[Space(10)]
	[Header("Inputs")]
	[Tooltip("how much you have to tilt your mobile device to fully activate airbrake")]
	public float tiltSensitivity = 1f;
	public float airBrakeSteer = 10f;
	public float deviceDeadzone = 0.25f;
	public KeyCode airBrakeLeft = KeyCode.X;
	public KeyCode airBrakeRight = KeyCode.C;
	private float airResistanceL;

	public GameObject explosionObject;
	public GameObject fireObject;
	//input values accessible from other scripts
	[HideInInspector]
	public float throttle;
	[HideInInspector]
	public float steer;
	[HideInInspector]
	public bool airBrakes;
	[HideInInspector]
	public bool boosting;
	[HideInInspector]
	public bool reversing;
	[HideInInspector]
	public bool shipDestroyed;
	[HideInInspector]
	public float deviceTilt;
	[HideInInspector]
	public bool sideShifting;
	Vector3 movingDirection;
	private ParticleSystem[] flames;
	private TrailRenderer[] airTrails;
	private Light[] flameGlows;
	private Rigidbody rb;

	Vector3 turnVec;
	Vector3 imp;
	Vector3 tempVEC;
	float actualTurn;
	float actualGrip;
	float slideSpeed;
	float currentSpeed;
	Vector3 flatVelo;

	float forwardDot;
	float sideDot;
	float upDot;
	Quaternion velDir;
	bool restoringEnergy;
	GameObject capsuleClone;
	GameObject explosionClone;
	GameObject fireClone;
	GameObject flightWindClone;
	GameObject sparkObject;
	int ShiftDirection;
	// Use this for initialization
	void Start () {
		energyLeft = 100f;
		healthLeft = 100f;
		rb = GetComponent<Rigidbody> ();
		flames = GetComponentsInChildren<ParticleSystem> ();
		airTrails = GetComponentsInChildren<TrailRenderer> ();
		flameGlows = GetComponentsInChildren<Light> ();
		GetComponent<AudioSource> ().clip = engineSound;
		GetComponent<AudioSource> ().spatialBlend = 1.0f;
		GetComponent<AudioSource> ().dopplerLevel = 0f;
		GetComponent<AudioSource> ().minDistance = 7f;
		GetComponent<AudioSource> ().loop = true;
		GetComponent<AudioSource> ().playOnAwake = true;
		GetComponent<AudioSource> ().volume = 1f;
		capsuleClone = Instantiate (healingCapsule, transform.position, Quaternion.identity) as GameObject;
		capsuleClone.transform.SetParent (transform);
		capsuleClone.SetActive (false);

		GameObject flightWind = new GameObject ("flightWind");
		flightWind.transform.position = transform.position;
		flightWind.transform.SetParent (transform);
		flightWind.AddComponent<AudioSource> ();
		flightWind.GetComponent<AudioSource> ().clip = flightWindSound;
		flightWind.GetComponent<AudioSource> ().spatialBlend = 1.0f;
		flightWind.GetComponent<AudioSource> ().dopplerLevel = 0f;
		flightWind.GetComponent<AudioSource> ().minDistance = 7f;
		flightWind.GetComponent<AudioSource> ().loop = true;
		flightWind.GetComponent<AudioSource> ().playOnAwake = true;
		flightWind.GetComponent<AudioSource> ().volume = 0f;
		flightWind.GetComponent<AudioSource> ().Play ();
		flightWindClone = flightWind;

		if (explosionObject != null && fireObject != null) {

			GameObject expl = (GameObject)Instantiate (explosionObject, transform.position, Quaternion.identity);
			expl.transform.parent = transform;
			expl.transform.localPosition = Vector3.zero;
			expl.SetActive (false);
			explosionClone = expl;

			GameObject fire = (GameObject)Instantiate (fireObject, transform.position, Quaternion.identity);
			fire.transform.parent = transform;
			fire.transform.localPosition = Vector3.zero;
			fire.SetActive (false);
			fireClone = fire;
		}

		if (collisionParticle != null) {
			GameObject crashParticle = (GameObject)Instantiate (collisionParticle, transform.position, transform.rotation);
			crashParticle.GetComponent<ParticleSystem> ().Stop ();
			sparkObject = crashParticle;

		}

	}

	void Update()
	{
		if (touchScreenControls) {
			deviceTilt = Input.acceleration.x * tiltSensitivity;
		}
	}


	bool doubleTapped = false;
	bool canSideshift = true;
	float ButtonCooler = 0.5f;
	int ButtonCount = 0;
	// Update is called once per frame
	void FixedUpdate () {
		//dot product between ship's local axes and the axis of gravity
		forwardDot = Vector3.Dot (transform.forward, Vector3.up);
		sideDot = Vector3.Dot (transform.right, Vector3.up);
		upDot = Vector3.Dot (transform.up, Vector3.up);
		healthLeft = Mathf.Clamp (healthLeft, 0f, 100f);
		energyLeft = Mathf.Clamp (energyLeft, 0f, 100f);
		CurrentSpeed = Mathf.Round (rb.velocity.magnitude * 3.6f);

		movingDirection = transform.InverseTransformDirection (rb.velocity);

		//if only in desktop mode input values will be assigned from this script
		if (!touchScreenControls) {

			throttle = Input.GetAxis ("Vertical");
			steer = Input.GetAxis ("Horizontal");
			boosting = Input.GetButton ("Fire1");

		}

		if (movingDirection.z < 0.5f && throttle < 0.01f)
			reversing = true;
		else
			reversing = false;

		if (healingCapsule != null) {
			if (restoringEnergy && healthLeft < 100 && !shipDestroyed) {
				healthLeft += (40) * Time.deltaTime;

				capsuleClone.SetActive (true);
				Vector2 offset = new Vector2 (0f, Mathf.Sin (Time.time * 10f) * 0.5f);
				capsuleClone.GetComponent<MeshRenderer> ().material.mainTextureOffset = offset;
			} else if (restoringEnergy && energyLeft < 100 && !shipDestroyed) {
				energyLeft += (25) * Time.deltaTime;

				capsuleClone.SetActive (true);
				Vector2 offset = new Vector2 (0f, Mathf.Sin (Time.time * 10f) * 0.5f);
				capsuleClone.GetComponent<MeshRenderer> ().material.mainTextureOffset = offset;
			} else {
				capsuleClone.SetActive (false);
				capsuleClone.GetComponent<MeshRenderer> ().material.mainTextureOffset = Vector2.zero;

			}
		}
		//boosting
		if (!boostIsHealth)
		{
			if (boosting && energyLeft > 0 && !reversing && CurrentSpeed > 20) {
				rb.AddForce (transform.forward * boostTorque * rb.mass * Time.deltaTime);
				energyLeft -= Time.deltaTime * boostConsumptionRate;

			} else {
				if (energyLeft < 100 && !boostIsHealth && autoBoostRegeneration)
					energyLeft += Time.deltaTime * boostRefillRate;
			}
		}
		else
		{
			if (boosting && healthLeft > 10 && !reversing && CurrentSpeed > 20) {
				rb.AddForce (transform.forward * boostTorque * rb.mass * Time.deltaTime);
				healthLeft -= Time.deltaTime * boostConsumptionRate;

			}


		}
		if (!shipDestroyed) {
			velDir = Quaternion.LookRotation (Vector3.up, rb.velocity);
			if (GetComponent<ThrusterScript> ().frontGrounded && GetComponent<ThrusterScript> ().backGrounded) {
				rb.angularDrag = 1f;
				if (!airBrakes)
					rb.drag = 1f;
				else {
					if (brakePower >= 1000f)
						rb.drag = 0.001f * brakePower;
					else
						rb.drag = 1f;
				}

				//downforce
				rb.AddForce ((-transform.up * Downforce * rb.velocity.magnitude));

				//forward motion
				if (throttle > 0.01f) {
					Vector3 forwardForce = transform.forward * accel * throttle;

					forwardForce = forwardForce * Time.deltaTime * rb.mass;

					rb.AddForce (forwardForce);
				}

				//reversing
				if (throttle < 0.01f && canReverse) {

					Vector3 reverseForce = transform.forward * reversePower * throttle;

					reverseForce = reverseForce * Time.deltaTime * rb.mass;

					rb.AddForce (reverseForce);

				}


			} else {
				rb.angularDrag = 1f;

				rb.drag = 0.1f;
				Vector3 flyForce = transform.forward * 1000 * Mathf.Abs(throttle);

				flyForce = flyForce * Time.deltaTime *rb.mass;

				rb.AddForce (flyForce);

			}

			//steering
			//the plane of steering needs to be in all 3 directions, because we can drive on walls and upside down and other crazy angles
			tempVEC = new Vector3 (rb.velocity.x, rb.velocity.y,rb.velocity.z);

			flatVelo = tempVEC;

			tempVEC = new Vector3 (Vector3.right.x, Vector3.up.y,Vector3.forward.z);
			
			slideSpeed = Vector3.Dot (transform.right, flatVelo);
			currentSpeed = flatVelo.magnitude;

			float airBrakeTilt = 0f;
			if (!touchScreenControls) {
				if ((Input.GetKey (airBrakeLeft) && Input.GetKey (airBrakeRight)) == false) {
					if (!touchScreenControls)
						airBrakes = false;

					if (Input.GetKey (airBrakeLeft)) {
						airBrakeTilt = 20f;
						airResistanceL = Mathf.Lerp (airResistanceL, Mathf.Abs (rb.velocity.magnitude / 100) * -(airBrakeSteer * 0.01f), Time.deltaTime * 5f);
					} else {
						airResistanceL = Mathf.Lerp (airResistanceL, 0f, Time.deltaTime * 5f);
					}

					if (Input.GetKey (airBrakeRight)) {
						airBrakeTilt = -20f;
						airResistanceL = Mathf.Lerp (airResistanceL, Mathf.Abs (rb.velocity.magnitude / 100) * (airBrakeSteer * 0.01f), Time.deltaTime * 5f);
					} else {
						airResistanceL = Mathf.Lerp (airResistanceL, 0f, Time.deltaTime * 5f);
					}
				} else {
					airBrakes = true;
					airBrakeTilt = 0f;
					airResistanceL = Mathf.Lerp (airResistanceL, 0f, Time.deltaTime * 5f);
				}
			} else {
			//use device accelerometer/tilt to activate air brakes
				if (Mathf.Abs (Input.acceleration.x) >= deviceDeadzone) {
					airBrakeTilt = -20f * Mathf.Clamp (deviceTilt, -1f, 1f);
					airResistanceL = Mathf.Abs (rb.velocity.magnitude / 100) * (airBrakeSteer * 0.01f)* deviceTilt;
				} else {
					airBrakeTilt = 0f;
					airResistanceL = Mathf.Lerp (airResistanceL, 0f, Time.deltaTime * 5f);
					airResistanceL = Mathf.Clamp (airResistanceL, -0.1f, 0.1f);
				}

			}

			turnVec = Vector3.up * (((rotateRate*0.01f) * steer) + airResistanceL)  * rb.mass;
			actualGrip = Mathf.Lerp (100, grip, currentSpeed * 0.02f);
			imp = transform.right * (-slideSpeed * rb.mass * actualGrip);

			Vector3 newRot = vehicleChildObj.localEulerAngles;
			newRot.z = Mathf.SmoothDampAngle (newRot.z, (-bodyRoll * steer) + airBrakeTilt, ref rotationVelocity, bodyRollSeekSpeed);
			vehicleChildObj.localEulerAngles = newRot;

				transform.Rotate(turnVec * Time.deltaTime);
				//force impulse to prevent sliding
			if (!sideShifting)
				rb.AddForce (imp * Time.deltaTime);
			if (sideShifting)
				rb.AddForce(transform.right * 1000f * ShiftDirection,ForceMode.Acceleration);
			//TODO add side shifting, air brakes, and speed pads
			if (!touchScreenControls)
				SideShift (Input.GetKeyDown (airBrakeLeft),Input.GetKeyDown (airBrakeRight));
			CorrectRotation ();

			//audio stuff

			flightWindClone.GetComponent<AudioSource> ().volume = Mathf.Clamp (rb.velocity.magnitude / 500, 0f, maxFlightWindVolume);
			flightWindClone.GetComponent<AudioSource> ().pitch = Mathf.Clamp (rb.velocity.magnitude / 100, 1.5f, 3.5f);

			if (!reversing) {
				if (throttle > 0.01f)
					GetComponent<AudioSource> ().pitch = Mathf.Lerp (GetComponent<AudioSource> ().pitch, Mathf.Lerp (minEnginePitch, maxEnginePitch, Mathf.Abs (throttle)), Time.deltaTime / (accel / 1000f));
				else
					GetComponent<AudioSource> ().pitch = Mathf.Lerp (GetComponent<AudioSource> ().pitch, minEnginePitch, Time.deltaTime * 3f);
				if (flames.Length > 0) {
					foreach (ParticleSystem flame in flames) {
						ParticleSystem.MainModule psMain = flame.main;
						psMain.startSpeed = Mathf.Lerp (psMain.startSpeed.constant, Mathf.Lerp (1.5f, 7.0f, Mathf.Abs (throttle)), Time.deltaTime * 2.5f);
					}
					foreach (Light light in flameGlows) {
						light.intensity = Mathf.Lerp (light.intensity, Mathf.Lerp (0.3f, 1.0f, throttle), Time.deltaTime * 2.5f);
					}
				}
			} else {
				GetComponent<AudioSource> ().pitch = Mathf.Lerp (GetComponent<AudioSource> ().pitch, minEnginePitch, Time.deltaTime * 2.5f);
				if (flames.Length > 0) {
					foreach (ParticleSystem flame in flames) {
						ParticleSystem.MainModule psMain = flame.main;
						psMain.startSpeed = Mathf.Lerp (psMain.startSpeed.constant, 1.5f, Time.deltaTime * 2.5f);
					}
					foreach (Light light in flameGlows) {
						light.intensity = Mathf.Lerp (light.intensity,0.3f,Time.deltaTime * 2.5f);
					}
				}

			}
			if (airTrails.Length > 0) {

				foreach (TrailRenderer tr in airTrails) {

					tr.time = Mathf.Clamp (CurrentSpeed / 100, 0f, 5f);

				}

			}
			//warning for low health
			if (fireObject != null && explosionObject != null) {
				if (healthLeft <= 15) {
					fireClone.SetActive (true);
				} else {
					fireClone.SetActive (false);
				}
			}


		} else {

			//reset after a few seconds
			StartCoroutine ("RespawnDelay");
		}

	}

	public void SideShift(bool left, bool right)
	{
		if (left) {
			if (ButtonCooler > 0f && ButtonCount == 1 && canSideshift) {
				//Has double tapped

				StartCoroutine ("sideShiftDelay", 2f);
				StartCoroutine ("sideShiftingBool", 0.1f);
			} else {
				
				ButtonCooler = 0.5f; 
				ButtonCount += 1;
			}
			ShiftDirection = -1;
		}

			if (ButtonCooler > 0) {
				ButtonCooler -= 1f * Time.deltaTime;

			} else {
				ButtonCount = 0;
			}
		
		if (right) {
			if (ButtonCooler > 0f && ButtonCount == 1 && canSideshift) {
				//Has double tapped
				StartCoroutine ("sideShiftDelay", 2f);
				StartCoroutine ("sideShiftingBool", 0.1f);
			} else {
				
				ButtonCooler = 0.5f; 
				ButtonCount += 1;
			}
			ShiftDirection = 1;
		}

			if (ButtonCooler > 0) {
				ButtonCooler -= 1f * Time.deltaTime;

			} else {
				ButtonCount = 0;
			}
		
	}

	IEnumerator sideShiftDelay(float delay)
	{
		canSideshift = false;
		yield return new WaitForSeconds (delay);
		canSideshift = true;
	}

	IEnumerator sideShiftingBool (float delay)
	{
		sideShifting = true;
		yield return new WaitForSeconds (delay);
		sideShifting = false;
	}


	IEnumerator RespawnDelay()
	{
		flightWindClone.GetComponent<AudioSource> ().volume = Mathf.Lerp (flightWindClone.GetComponent<AudioSource>().volume,0f,Time.deltaTime);
		GetComponent<AudioSource> ().pitch = Mathf.Lerp (GetComponent<AudioSource> ().pitch, 0f, Time.deltaTime / 2);
		foreach (ParticleSystem flame in flames) {
			ParticleSystem.MainModule psMain = flame.main;
			psMain.startSpeed = Mathf.Lerp (psMain.startSpeed.constant, 0f, Time.deltaTime * 4f);
			//StartCoroutine ("turnOffEngines");
		}
		foreach (Light light in flameGlows) {
			light.intensity = Mathf.Lerp (light.intensity,0f,Time.deltaTime * 4f);
		}
		GetComponent<ThrusterScript> ().enabled = false;
		yield return new WaitForSeconds (1f);
		foreach (ParticleSystem flame in flames) {
			flame.gameObject.SetActive (false);
		}
		explosionClone.SetActive (true);
		fireClone.SetActive (true);
		yield return new WaitForSeconds (3f);

		foreach (ParticleSystem flame in flames) {
			flame.gameObject.SetActive (true);
		}
		GetComponent<ThrusterScript> ().enabled = true;
		shipDestroyed = false;
		healthLeft += Time.deltaTime * 10;
		explosionClone.SetActive (false);
		fireClone.SetActive (false);
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
			if (Vector3.Dot(hit.normal, Vector3.up) >= 0.1f)
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
				/*	GetComponent<Rigidbody>().velocity.sqrMagnitude > 10 ? */Vector3.Dot(transform.forward, velDir * Vector3.right) * Mathf.Abs(actualUpDir) * rotationCorrectionPower.y - transform.InverseTransformDirection(rb.angularVelocity).y * Mathf.Pow(actualUpDir, 2) * 10,
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
				if (healthLeft > 0) {
					ApplyDamage ((other.impulse / Time.fixedDeltaTime).magnitude / rb.mass / 10f);
				} else {
					shipDestroyed = true;
					if (explosionObject != null && fireObject != null) {
						explosionClone.SetActive (true);
						fireClone.SetActive (true);
					}
				}


			}
			if (collisionParticle != null) {
				if (other.transform != transform && other.contacts.Length != 0 && CurrentSpeed >= 50f) {

					foreach (ContactPoint cp in other.contacts) {
						sparkObject.transform.position = cp.point;
						sparkObject.transform.rotation = Quaternion.LookRotation (other.relativeVelocity.normalized, cp.normal);
						StartCoroutine (sparkDisable ());

					}
				} else {
					sparkObject.GetComponent<ParticleSystem> ().Stop ();

				}

			}
		}

	}
	IEnumerator sparkDisable()
	{
		yield return new WaitForSeconds (1);
		sparkObject.GetComponent<ParticleSystem> ().Stop ();
	}

	void OnCollisionStay(Collision other)
	{
		//spawns collision particles when scraping against a wall
		if (collisionParticle != null)
		{


			if (other.transform != transform && other.contacts.Length != 0) {

				if (CurrentSpeed > 60f) {
					foreach (ContactPoint cp in other.contacts) {

						sparkObject.transform.position = cp.point;
						sparkObject.transform.rotation = Quaternion.LookRotation (other.relativeVelocity.normalized, cp.normal);
						sparkObject.GetComponent<ParticleSystem> ().Play ();
						sparkObject.transform.rotation = transform.rotation;

					}
				} else {
					sparkObject.GetComponent<ParticleSystem> ().Stop ();
				}
			}
		}
	}
		
	void OnTriggerStay (Collider other)
	{

		if (other.transform.tag == "healingPad") {
			restoringEnergy = true;

		}
		if (other.tag == "BoostPad") {
			rb.AddForce (other.transform.forward * 500f,ForceMode.Acceleration);
		}

	}
	void OnTriggerExit (Collider other)
	{

		if (other.transform.tag == "healingPad") {
			restoringEnergy = false;

		}

	}
	public void ApplyDamage (float damage)
	{


		healthLeft -= damage / strength;	

	}



}

