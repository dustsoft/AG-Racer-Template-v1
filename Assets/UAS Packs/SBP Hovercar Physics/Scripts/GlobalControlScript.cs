using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GlobalControlScript : MonoBehaviour {

	ShipController controller;
	public Text speedText;
	public Image healthBar;
	public Image energyBar;
	public GameObject healthBarContainer;
	public GameObject energyBarContainer;
	public GameObject FollowCamera;
	public GameObject LevelCamera;
	public GameObject options;
	public GameObject gameHUD;
	public GameObject menuHUD;
	public Toggle useHealth;
	public Toggle healthIsBoost;
	public Toggle autoBoostRegen;
	[Space(20)]
	public GameObject ship1;
	public GameObject ship2;
	public GameObject ship3;
	public GameObject ship4;
	public GameObject ship5;
	public GameObject ship6;
	Color originalHealthBarColor;
	// Use this for initialization
	void Start () {
		originalHealthBarColor = healthBar.color;
		ship1.SetActive (false);
		ship2.SetActive (false);
		ship3.SetActive (false);
		ship4.SetActive (false);
		ship5.SetActive (false);
		ship6.SetActive (false);
		FollowCamera.SetActive (false);
		LevelCamera.SetActive (true);
		menuHUD.SetActive (true);
		gameHUD.SetActive (false);
		options.SetActive (false);
		GameObject player_ = GameObject.FindGameObjectWithTag ("Player");
		if (!player_)
			return;
		
			controller = player_.GetComponent<ShipController>();
	}
	Color spdColr;
	// Update is called once per frame
	void Update () {
		GameObject player_ = GameObject.FindGameObjectWithTag ("Player");
		if (!player_)
			return;
		
			controller = player_.GetComponent<ShipController>();
		
		speedText.text = controller.CurrentSpeed.ToString();

		healthBar.fillAmount = Mathf.Clamp (controller.healthLeft/100, 0f, 1f);
		energyBar.fillAmount = Mathf.Clamp (controller.energyLeft/100, 0f, 1f);

		controller.useHealth = useHealth.isOn;
		controller.boostIsHealth = healthIsBoost.isOn;
		controller.autoBoostRegeneration = autoBoostRegen.isOn;

		if (controller.boosting && controller.energyLeft > 0f)
		{
			spdColr = Color.Lerp (spdColr, Color.blue, Time.deltaTime * 5);
		}
		else
		{
			spdColr = Color.Lerp(spdColr,Color.white,Time.deltaTime * 5);
		}
		speedText.color = spdColr;

		if (controller.useHealth)
			healthBarContainer.SetActive (true);
		else
			healthBarContainer.SetActive (false);

		if (controller.boostIsHealth) {
			energyBarContainer.SetActive (false);
		} else {
			energyBarContainer.SetActive (true);
		}

		if (controller.healthLeft <= 15) {
		
			Color warningColor = healthBar.color;
			warningColor.r = Mathf.PingPong (Time.time*3f, 1f);
			healthBar.color = warningColor;
		} else {
		
			healthBar.color = originalHealthBarColor;
		
		}


	}

	public void Ship1()
	{
		ship1.SetActive (true);
		ship2.SetActive (false);
		ship3.SetActive (false);
		ship4.SetActive (false);
		ship5.SetActive (false);
		ship6.SetActive (false);
		LevelCamera.SetActive (false);
		FollowCamera.SetActive (true);
		menuHUD.SetActive (false);
		gameHUD.SetActive (true);
		options.SetActive (false);
	}

	public void Ship2()
	{
		ship1.SetActive (false);
		ship2.SetActive (true);
		ship3.SetActive (false);
		ship4.SetActive (false);
		ship5.SetActive (false);
		ship6.SetActive (false);
		LevelCamera.SetActive (false);
		FollowCamera.SetActive (true);
		menuHUD.SetActive (false);
		gameHUD.SetActive (true);
		options.SetActive (false);
	}

	public void Ship3()
	{
		ship1.SetActive (false);
		ship2.SetActive (false);
		ship3.SetActive (true);
		ship4.SetActive (false);
		ship5.SetActive (false);
		ship6.SetActive (false);
		LevelCamera.SetActive (false);
		FollowCamera.SetActive (true);
		menuHUD.SetActive (false);
		gameHUD.SetActive (true);
		options.SetActive (false);
	}

	public void Ship4()
	{
		ship1.SetActive (false);
		ship2.SetActive (false);
		ship3.SetActive (false);
		ship4.SetActive (true);
		ship5.SetActive (false);
		ship6.SetActive (false);
		LevelCamera.SetActive (false);
		FollowCamera.SetActive (true);
		menuHUD.SetActive (false);
		gameHUD.SetActive (true);
		options.SetActive (false);
	}

	public void Ship5()
	{
		ship1.SetActive (false);
		ship2.SetActive (false);
		ship3.SetActive (false);
		ship4.SetActive (false);
		ship5.SetActive (true);
		ship6.SetActive (false);
		LevelCamera.SetActive (false);
		FollowCamera.SetActive (true);
		menuHUD.SetActive (false);
		gameHUD.SetActive (true);
		options.SetActive (false);
	}

	public void Ship6()
	{
		ship1.SetActive (false);
		ship2.SetActive (false);
		ship3.SetActive (false);
		ship4.SetActive (false);
		ship5.SetActive (false);
		ship6.SetActive (true);
		LevelCamera.SetActive (false);
		FollowCamera.SetActive (true);
		menuHUD.SetActive (false);
		gameHUD.SetActive (true);
		options.SetActive (false);
	}

	public void OpenOptions()
	{
		options.SetActive (true);
	}
	public void CloseOptions()
	{
		options.SetActive (false);
	}

	public void OpenMenu()
	{
		ship1.SetActive (false);
		ship2.SetActive (false);
		ship3.SetActive (false);
		ship4.SetActive (false);
		ship5.SetActive (false);
		ship6.SetActive (false);
		LevelCamera.SetActive (true);
		FollowCamera.SetActive (false);
		menuHUD.SetActive (true);
		gameHUD.SetActive (false);
		options.SetActive (false);
	}
}
