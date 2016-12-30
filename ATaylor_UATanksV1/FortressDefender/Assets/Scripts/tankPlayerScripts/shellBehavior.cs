using UnityEngine;
using System.Collections;

public class shellBehavior : MonoBehaviour {
	public float throwForce;
	private float existTime;
	public float shellTimeOut;

	// Use this for initialization
	void Start () {
		throwForce = GameManager.instance.sForce;
		shellTimeOut = GameManager.instance.timeOutOfShell;
		gameObject.GetComponent<Rigidbody> ().velocity = transform.forward * throwForce;
	}
	
	// Update is called once per frame
	void Update () {
	
		existTime = existTime + Time.deltaTime;
		if (existTime >= shellTimeOut) 
		{//ensures shells are destroyed after timeout period
			Destroy(this.gameObject);
		}
	}
	void OnCollisionEnter(Collision Other)
	{
		if (Other.gameObject.tag == "enemyTank") {
			Other.gameObject.GetComponent<AIController> ().hit (this.gameObject.tag);//subtracts health
			Destroy(this.gameObject);//ensures shell is destroyed on impact


		}
		if (Other.gameObject.tag == "playerOne") {
			Other.gameObject.GetComponent<InputController> ().hit (this.gameObject.tag);//subtracts health and lives from player
			Destroy(this.gameObject);//ensures shell is destroyed on impact

		}
		if (Other.gameObject.tag == "playerTwo") {
			Other.gameObject.GetComponent<InputController> ().hit (this.gameObject.tag);//subtracts health and lives from player
			Destroy (this.gameObject);//ensures shell is destroyed on impact
		}
	}
}
