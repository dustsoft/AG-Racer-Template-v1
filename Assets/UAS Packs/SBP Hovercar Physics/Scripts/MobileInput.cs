using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MobileInput : MonoBehaviour{
	//remember to check the mobile input checkbox
	//also remember to mark all your enabled player objects as the "Player" tag
	public ShipController controller;
	public float accel;
	public float steer;
	public bool boost;
	public bool airBrakes;
	//double tap left or right steer button to side shift
	public bool shiftL;
	public bool shiftR;

	float a1;
	float s1;

	public void SetAccel(float a)
	{
		a1 = a;
	}

	public void SetSteer(float s)
	{
		s1 = s;
	}


	//detect double tap on left steer button
	public void SteerL()
	{
		StartCoroutine (clickWait ());
	}
	IEnumerator clickWait()
	{
		shiftL = true;
		yield return new WaitForFixedUpdate();
		shiftL = false;
	}
	public void SteerLReset()
	{
		shiftL = false;

	}


	//detect double tap on right steer button
	public void SteerR()
	{
		StartCoroutine (clickWait2 ());

	}
	IEnumerator clickWait2()
	{
		shiftR = true;
		yield return new WaitForFixedUpdate();
		shiftR = false;
	}
	public void SteerRReset()
	{
			shiftR = false;

	}


	public void Steer(bool s)
	{
		shiftL = s;
	}
	public void AirBrakes(bool ab)
	{
		airBrakes = ab;
	}

	public void Boost(bool b)
	{
		boost = b;
	}



	void FixedUpdate()
	{
		GameObject player_ = GameObject.FindGameObjectWithTag ("Player");
		if (!player_)
			return;

		controller = player_.GetComponent<ShipController>();

		if (s1 != 0)
			steer += Time.deltaTime * 3 * s1;
		else
			steer = Mathf.Lerp (steer, 0, Time.deltaTime * 10);

		if (a1 != 0)
			accel += Time.deltaTime * 3 * a1;
		else
			accel = Mathf.Lerp (accel, 0, Time.deltaTime * 10);


		accel = Mathf.Clamp (accel, -1f, 1f);
		steer = Mathf.Clamp (steer, -1f, 1f);
		controller.throttle = accel;
		controller.steer = steer;
		controller.airBrakes = airBrakes;
		controller.boosting = boost;
		controller.SideShift (shiftL, shiftR);
	}
}
