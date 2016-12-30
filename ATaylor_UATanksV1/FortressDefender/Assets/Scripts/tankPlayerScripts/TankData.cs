using UnityEngine;
using System.Collections;

public class TankData : MonoBehaviour {
	public float moveFSpeed = 5; // in meters per second,forward speed
	public float moveRSpeed = 5;//in meters per second, reverse speed
	public float rotateSpeed = 180; // in degrees per second
	public int fireRate=5;//rate at which tank can shoot
	public float shellForce=500;//force at which shell is fired from tank
	public int damageAmount=10;//amount of damage that a shell fired does on impact
	public int maxHealth=350;//health of tank, when it reaches <=0 tank dies
	public float currentHealth=350;//curent health of tank
	public int pointValue=5;//point value of killing enemy tank
	public int playerScore=0;//player's score
	public float hearingDistance=4.0f;//distance at which tank can hear
	public float seeingDistance=2.0f;//distance at which tank can see
	public float inSightsAngle=1.0f;//distance at which something is in sights of tank
	public float avoidanceTime = 2.0f;//time that tank will attempt to avoid obstacles before moving forward
	public float restingTime=10.0f;//time that tank will rest to heal
	public float fleeingDistance=10.0f;//distance than an enemy tank will attempt to flee
	public float hpHealOverTick=10;//hp that tank will heal upon tick
	public Transform[] patrolPoints;//waypoints tank will travel over
	public float closeEnough=2;//determines if tank is close enough to target
	public int personalityChosen;//behaviour of tank based on their personality
	private SphereCollider hearingRadius;//adjust collider radius based on hearingDistance
	public Transform trans;//used to get transform of tank
	public Vector3 shellSize;//used to resize bullets
	public int lives;
	public AudioClip deathSound;
	public AudioClip impactSound;
	public AudioClip shootSound;
	// Use this for initialization
	void Start () {
		lives = GameManager.instance.lives;
		shellSize=new Vector3(0.3f,0.3f,0.3f);
		trans = gameObject.GetComponent<Transform> ();
		moveFSpeed = GameManager.instance.mForwardSpeed;
		moveRSpeed = GameManager.instance.mBackwardSpeed;
		rotateSpeed = GameManager.instance.rotationSpeed;
		fireRate = GameManager.instance.fRate;
		shellForce = GameManager.instance.sForce;
		damageAmount = GameManager.instance.shellDamageAmount;
		maxHealth = GameManager.instance.hp;
		currentHealth = GameManager.instance.hp;
		pointValue = GameManager.instance.pointsWorth;
		hearingDistance = GameManager.instance.hearingRadius;
		hearingRadius = transform.GetComponent<SphereCollider> ();//ensures collider is changed based on hearingDistance specified by designers
		hearingRadius.radius = hearingDistance;
		seeingDistance = GameManager.instance.seeingDistance;
		inSightsAngle = GameManager.instance.playerInSightDistance;
		avoidanceTime = GameManager.instance.avoidanceTime;
		fleeingDistance = GameManager.instance.fleeingDistance;
		restingTime = GameManager.instance.restingTime;
		hpHealOverTick = GameManager.instance.hpHealOverTick;
		closeEnough = GameManager.instance.closeEnough;
		deathSound = audioList.instance.death;
		impactSound = audioList.instance.impact;
		shootSound = audioList.instance.shooting;
		if (gameObject.name == "enemyTank") {//enemyTank gets personality1 selected by designer
			if (GameManager.instance.personality1 == GameManager.tankPersonality.Explorer) {
				personalityChosen = 0;
			} else if (GameManager.instance.personality1 == GameManager.tankPersonality.Sporadic) {
				personalityChosen = 1;
			} else if (GameManager.instance.personality1 == GameManager.tankPersonality.Aggressive) {
				personalityChosen = 2;
			} else {
				personalityChosen = 3;
			}
		}
		 else if (gameObject.name == "enemyTank 1") {//enemyTank 1 gets personality2 selected by designer
			if (GameManager.instance.personality2 == GameManager.tankPersonality.Explorer) {
				personalityChosen = 0;
			} else if (GameManager.instance.personality2 == GameManager.tankPersonality.Sporadic) {
				personalityChosen = 1;
			} else if (GameManager.instance.personality2 == GameManager.tankPersonality.Aggressive) {
				personalityChosen = 2;
			} else {
				personalityChosen = 3;
			}
		} else if (gameObject.name == "enemyTank 2") {//enemyTank 2 gets personality3 selected by designer
			if (GameManager.instance.personality3 == GameManager.tankPersonality.Explorer) {
				personalityChosen = 0;
			} else if (GameManager.instance.personality3 == GameManager.tankPersonality.Sporadic) {
				personalityChosen = 1;
			} else if (GameManager.instance.personality3 == GameManager.tankPersonality.Aggressive) {
				personalityChosen = 2;
			} else {
				personalityChosen = 3;
			}
		} else {//all other ai tank names will receive personality 4 selected by designer
			if (GameManager.instance.personality4 == GameManager.tankPersonality.Explorer) {
				personalityChosen = 0;
			} else if (GameManager.instance.personality4 == GameManager.tankPersonality.Sporadic) {
				personalityChosen = 1;
			} else if (GameManager.instance.personality4 == GameManager.tankPersonality.Aggressive) {
				personalityChosen = 2;
			} else {
				personalityChosen = 3;
			}
		}
		if (gameObject.name == "playerTank") {
			hearingRadius.radius = 0;
		}
		if (gameObject.name == "player2Tank") {
			hearingRadius.radius = 0;
		}

	}
	
	// Update is called once per frame
	void Update () {

	
	}

}
