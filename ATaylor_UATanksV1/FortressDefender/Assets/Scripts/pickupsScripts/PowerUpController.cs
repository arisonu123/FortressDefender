using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PowerUpController : MonoBehaviour {
	private bool canUse;
	public List<powerups> powerupsList;
	public TankData data;
	// Use this for initialization

	void Start () {
		powerupsList=new List<powerups>();
		if (data == null) 
		{
			data=gameObject.GetComponent<TankData>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		List<powerups> expiredPowerups = new List<powerups>();//list to keep track of expired powerups(powerups that duration has hit 0)
		// Loop through all the powers in the List
		foreach (powerups power in powerupsList)
		{
			// Subtract from the timer
			power.duration -= Time.deltaTime;
			
			// Assemble a list of expired powerups
			if (power.duration <= 0) {
				expiredPowerups.Add (power);
			}
		}

		// remove each expired powerup from our powerupsList.
		foreach (powerups power in expiredPowerups) {
			power.onDeactivate(data);
			powerupsList.Remove(power);
		}
		expiredPowerups.Clear ();//clear list
	}   
	

	public void addPowerup(powerups powerup){//adds powerups to list as they are picked up
		powerupsList.Add (powerup);
		powerup.onActivate (data);


	}
}
