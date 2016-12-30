using UnityEngine;
using System.Collections;
[System.Serializable]//serializes class so that elements may be seen in the inspecter
public class powerups{//powerup stat modifiers
	public float healthModifier;
	public Vector3 shellSizeModifier;
	public int fireRateModifier;
	public float speedModifier;
	public float duration;
	public bool isPermanent;
	private Vector3 zeroVector = new Vector3 (0, 0, 0);
	public void onActivate(TankData  currentData)
	{   


		if(currentData.currentHealth!=currentData.maxHealth)
	    {
		currentData.currentHealth = (currentData.currentHealth+healthModifier);
		}


	
		currentData.fireRate = (currentData.fireRate - fireRateModifier);
		currentData.moveFSpeed = (currentData.moveFSpeed + speedModifier);
		if(shellSizeModifier!=zeroVector){//only changes shellSize if shellSizeModifier is not equal to(0,0,0)
		currentData.shellSize = shellSizeModifier;
		}
	}
	public void onDeactivate(TankData currentData)
	{

		currentData.fireRate = (currentData.fireRate + fireRateModifier);
		currentData.moveFSpeed = (currentData.moveFSpeed - speedModifier);
        if (shellSizeModifier != zeroVector) {
			currentData.shellSize = new Vector3 (0.3f, 0.3f, 0.3f);
		}
 
	}
}
