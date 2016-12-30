using UnityEngine;
using System.Collections;

public class pickup : MonoBehaviour 
{
	public powerups powerup;
	public AudioClip feedback; 
	public Transform trans;
	// Use this for initialization
	void Start () {
		trans = gameObject.GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void OnTriggerEnter(Collider other)
	{
		//variable to store other object's PowerUpController- if it has one
		PowerUpController powCon = other.GetComponent<PowerUpController> ();
		//if other object has powerUpController
		if (powCon != null) {
			//Add the powerup
			powCon.addPowerup (powerup);
			// Play Feedback (if it is set)
			if (feedback != null) {
				AudioSource.PlayClipAtPoint (feedback, trans.position, GameManager.instance.sfxVolume);
			}
			//destroy this pickup
			Destroy (gameObject);
		}
	}
	
}
