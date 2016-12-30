using UnityEngine;
using System.Collections;
[RequireComponent (typeof(TankData))]//requires TankData
public class InputController : MonoBehaviour {
	public enum InputScheme { WASD, arrowKeys };
	public InputScheme playerInput=InputScheme.WASD;//set default control scheme 
	public AudioClip deathSound;
	public AudioClip impactSound;
	public tankMotor motor;
	public TankData data;
	private float lastShot;//used to keep track of last time shell was shot, used for controlling firing rate
	private Transform playerPos;
	// Use this for initialization
	void Start () {
		if (motor == null)//sets motor if it is currently null
		{
			motor = gameObject.GetComponent<tankMotor>();
		}
		if (data == null)//sets data if it is currently null
		{
			data = gameObject.GetComponent<TankData>();
		}

		lastShot = 0f;
		playerPos = gameObject.transform;
		deathSound = audioList.instance.death;
		impactSound = audioList.instance.impact;
	}
	
	// Update is called once per frame
	void Update () {
		switch (playerInput) {//moves player based on input from the p1input scheme
		case InputScheme.WASD:
			if (Input.GetKey(KeyCode.W)){//moves player forward when W is pressed
				motor.Move(data.moveFSpeed);
			}
			else if (Input.GetKey(KeyCode.S)){//moves player in reverse when S is pressed
				motor.Move(-data.moveRSpeed);
			}
		    else{//if neither moving forward or backward stop
				motor.Stop ();
			}
			if (Input.GetKey(KeyCode.D)){//moves player right when D is pressed
				motor.Rotate(data.rotateSpeed);
			}
			if (Input.GetKey(KeyCode.A)){//moves player left when A is pressed
				motor.Rotate(-data.rotateSpeed);
			}
			if(Input.GetKey(KeyCode.Space)){//shoots when space is pressed
				//shooterName=this.gameObject.tag;
				if(lastShot>=data.fireRate)//checks if enough time has passed since last shell shot
		//completes action/fires if so
				{

					gameObject.GetComponent<shoot>().shootShell(data.shellSize);
					lastShot=0f;
				}
			}
			break;
		case InputScheme.arrowKeys:
			if (Input.GetKey(KeyCode.UpArrow)){//moves player forward when the up arrow is pressed
				motor.Move(data.moveFSpeed);
			}
			else if (Input.GetKey(KeyCode.DownArrow)){//moves player in reverse when the down arrow is pressed
				motor.Move(-data.moveRSpeed);
			}
			else{//if not moving forward or back then stop
				motor.Stop ();
			}
			if (Input.GetKey(KeyCode.RightArrow)){//moves player right when the right arrow is pressed
				motor.Rotate(data.rotateSpeed);
			}
			if (Input.GetKey(KeyCode.LeftArrow)){//moves player left when the left arrow is pressed
				motor.Rotate(-data.rotateSpeed);
			}

			if(Input.GetKey(KeyCode.RightControl)){//shoots when space is pressed
				//shooterName=this.gameObject.tag;(used to make sure right object is firing
				if(lastShot>=data.fireRate)//checks if enough time has passed since last shell shot
					//completes action/fires if so
				{

	 
					gameObject.GetComponent<shoot>().shootShell(data.shellSize);
					lastShot=0f;//sets time that last shot happened to Time.time or time since program/game started
				}
			}
			break;
		}
		lastShot=lastShot+Time.deltaTime;//increments time since last shot
	}
	public void hit(string shooter)//used to subtract health
	{
		AudioSource.PlayClipAtPoint(impactSound,playerPos.transform.position,GameManager.instance.musicVolume);
        if (shooter != "playerTank(Clone)" && shooter != "player2Tank(Clone)") {
			data.currentHealth = data.currentHealth - data.damageAmount;
		}
		if (data.currentHealth <= 0) {//destroys tank
			AudioSource.PlayClipAtPoint(deathSound,playerPos.position,GameManager.instance.musicVolume);
			data.lives=data.lives-1;
			destroyTank();
			GameManager.instance.reSpawn();
		}
		
		
	}
	public void destroyTank()//used to destroy/deactivate a tank
	{
		Transform visualObj=gameObject.transform.Find ("Visuals");
		Transform BodyObj=visualObj.transform.Find ("Body");
		Transform CannonObj = visualObj.transform.Find ("Cannon");
		Renderer[] visuals = BodyObj.GetComponentsInChildren<Renderer> ();
		Renderer[] turretVis = CannonObj.GetComponentsInChildren <Renderer> ();
		foreach (Renderer visual in visuals)//enable all renderers so tank is invisible but scripts are still active
		{
			visual.enabled=false;
		}
		foreach (Renderer visual in turretVis)//enable all renderers so tank is invisible but scripts are still active
		{
			visual.enabled=false;
		}
		gameObject.GetComponent<SphereCollider> ().enabled = false;
		gameObject.GetComponentInChildren<Camera> ().enabled = false;
		gameObject.GetComponent<CharacterController> ().enabled = false;
	}
	public void tankRespawn(){//used to reactivate a tank/respawn it
		gameObject.SetActive (true);
		Transform visualObj=gameObject.transform.Find ("Visuals");
		Transform BodyObj=visualObj.transform.Find ("Body");
		Transform CannonObj = visualObj.transform.Find ("Cannon");
		Renderer[] visuals = BodyObj.GetComponentsInChildren<Renderer> ();
		Renderer[] turretVis = CannonObj.GetComponentsInChildren <Renderer> ();
		foreach (Renderer visual in visuals)//enable all renderers so tank is invisible but scripts are still active
		{
			visual.enabled=true;
		}
		foreach (Renderer visual in turretVis)//enable all renderers so tank is invisible but scripts are still active
		{
			visual.enabled=true;
		}
		gameObject.GetComponentInChildren<Camera> ().enabled = true;
		gameObject.GetComponent<SphereCollider> ().enabled = false;

		gameObject.GetComponent<CharacterController> ().enabled = true;
		data.currentHealth = data.maxHealth;//sets tanks hp back to max upon respawning
	}
}
