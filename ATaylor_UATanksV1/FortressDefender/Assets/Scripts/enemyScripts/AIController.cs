using UnityEngine;
using System.Collections;
using System.Runtime;
[RequireComponent (typeof(TankData))]//requires TankData
[RequireComponent (typeof(tankMotor))]//requires TankData
public class AIController : MonoBehaviour {
	private tankMotor motor;
	private TankData data;//default tank data
	private TankData currentTankData;//ensures changes can be made to current tank data without changing for all
	private Transform trans;
	public int avoidanceStage = 0;
	private float exitTime;
	private float timeSinceShot;//time since shot was last fired
	public int currentWaypoint=0;
	private bool isPatrolForward = true;//keeps track of whether or not tank is patrolling forward
	public Transform[] patrolPoints;
	public float closeEnough = 1.0f;//determines how close is close enough to waypoint
	public enum AIState { Chase, ChaseAndFire, CheckForFlee, Flee, Rest,Patrol};
	public AIState aiState = AIState.Patrol;
	public float stateEnterTime;
	public float aiSenseRadius;//how close is close enough for tank to hear
	public float FOVsight;//how close is close enough to be in field of view of enemy tank
	public float playerInSightsFOV;//how close is close to be in sights of enemy tank
	public float restingHealRate; // in hp/second 
	public float fleeingDistance=1.0f;//distance a fleeing ai will run
	public float avoidanceTime = 2.0f;//time that tank spends avoiding
	private int personalityChosen;
	private System.Random rngDecision = new System.Random();//System prefix is used to get System version rather than UnityEngine version
	public Transform playerLoc;//gets transform of player 1 location
	public Transform playerTwoLoc;//gets transform of player 2 location
	public Transform targetPlayer;//used for purpose of changing states and getting the right player target
	public GameObject[] enemyTanks;//whole array of enemy tanks
	public GameObject currentEnemyTank;//used to ensure changes are being made to correct tank
	private bool increasedSpeed;
	public Texture personalityLook0;//used to load diffrent textures based on personality
	public Texture personalityLook1;
	public Texture personalityLook2;
	public Texture personalityLook3;
	public AudioClip deathSound;
	public AudioClip impactSound;


	void Awake()
	{
		trans = gameObject.GetComponent<Transform> ();


	}


	// Use this for initialization
	void Start () {
		if (data == null)//sets data if it is currently null
		{
			data = gameObject.GetComponent<TankData>();
		}
		if (motor == null) {
			motor = gameObject.GetComponent<tankMotor> ();
		}
		if(currentTankData==null){//ensures current tank data can be gotten,important to make modifications only to current one and not all instances of TankData
			currentTankData=currentEnemyTank.GetComponent<TankData>();
		}
		deathSound = audioList.instance.death;
		impactSound = audioList.instance.impact;
		avoidanceTime = GameManager.instance.avoidanceTime;
		patrolPoints = currentTankData.patrolPoints;
		closeEnough = data.closeEnough;
		avoidanceTime = data.avoidanceTime;
		restingHealRate = data.hpHealOverTick;
		aiSenseRadius = data.hearingDistance;
		FOVsight = data.seeingDistance;
		playerInSightsFOV = data.inSightsAngle;
		enemyTanks = GameManager.instance.enemies;
		playerLoc = GameManager.instance.players[0].transform;//gets transform of player one for location purposes
		playerTwoLoc = GameManager.instance.players [1].transform;//gets transform of player two for location purposes
		timeSinceShot = data.fireRate;
		personalityChosen = data.personalityChosen;
		fleeingDistance = data.fleeingDistance;
		increasedSpeed = false;
		Renderer[] look=gameObject.GetComponentsInChildren<Renderer>();
		if (personalityChosen == 0) {//decides look/texture based on tank personality


			for(int i=0;i<look.Length-1;i++){
				look[i].material.mainTexture=personalityLook0;
			}
		} else if (personalityChosen == 1) {

			for(int i=0;i<look.Length-1;i++){
				look[i].material.mainTexture=personalityLook1;
			}
		} else if (personalityChosen == 2) {

			for(int i=0;i<look.Length-1;i++){
				look[i].material.mainTexture=personalityLook2;
			
			}
		} else {

			for(int i=0;i<look.Length-1;i++){
				look[i].material.mainTexture=personalityLook3;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(aiState==AIState.Patrol)//perform behaviors for Patrol state
		{
			//Perform behaviors
			if(avoidanceStage!=0){//makes sure tank is avoiding other enemy tanks and obstacles
				DoAvoidance();
			}
			//Check for state transitions
			else if ((Vector3.SqrMagnitude(trans.position-playerLoc.position) <=(aiSenseRadius*aiSenseRadius)&&personalityChosen==0)||(Vector3.SqrMagnitude(trans.position-playerLoc.position) <=(aiSenseRadius*aiSenseRadius)&&personalityChosen==2)){//if can hear playerOne,switch to chase state
				targetPlayer=playerLoc;
				ChangeState(AIState.Chase);  //playerOne can be heard if they are in hearing range/sphere collider of enemy tank
			}
			else if ((Vector3.SqrMagnitude(trans.position-playerTwoLoc.position) <=(aiSenseRadius*aiSenseRadius)&&personalityChosen==0)||(Vector3.SqrMagnitude(trans.position-playerTwoLoc.position) <=(aiSenseRadius*aiSenseRadius)&&personalityChosen==2)){//if can hear playerTwo,switch to chase state
				targetPlayer=playerTwoLoc;
				ChangeState(AIState.Chase);    //playerTwo can be heard if they are in hearing range/sphere collider of enemy tank
			}
			else if(CanSee(playerLoc.position)==true&&personalityChosen==1||CanSee (playerTwoLoc.position)==true&&personalityChosen==1)//if either player can be seen then sporadic tank commences chase
			{
				ChangeState(AIState.Chase);
			}
			else if(CanSee(playerLoc.position)==true&&personalityChosen==3||CanSee(playerTwoLoc.position)==true&&personalityChosen==3)//if quiet tank can see either player and another enemy tank's hp is less than 100 will chase the player
			{
				for (int i=0;i<=enemyTanks.Length-1;i++)//checks all enemy tanks hp to see if it is low
				{
					float hp=enemyTanks[i].gameObject.GetComponent<TankData>().currentHealth;
					if (hp<100)
					{

						ChangeState(AIState.Chase);//if another enemy tank is low then the quiet tank will chase the player as long as it can see
						break;//breaks out of for loop
					}
				}

			}

		
			else{
				DoPatrol ();
			}
			timeSinceShot=timeSinceShot+Time.deltaTime;//increments time since last shot
		}
		else if ( aiState == AIState.Chase )
		{
			// Perform Behaviors For Chase State
			if (avoidanceStage != 0) {
				DoAvoidance();
			} else {
				DoChase();
			}
			
			// Check for Transitions

			if((data.currentHealth<100&&personalityChosen==0)||(CanSee(targetPlayer.position)==false&&personalityChosen==0)) //if explorer tank cannot see target player after stateEnterTime+10 or hp is <100 Check For Flee state switch
			{
				if (Time.time >= stateEnterTime + 10) {
					ChangeState(AIState.CheckForFlee);
				}
			}

			else if(personalityChosen==1)//decide if sporadic tank will flee or continue the chase
			{
				int willFlee=rngDecision.Next(0,11);
				if(willFlee>=6){
				//do nothing
				}
				else if(willFlee<6){//sporadic tank will now flee
				ChangeState (AIState.CheckForFlee);
				}
			}

			else if (Vector3.SqrMagnitude(trans.position-targetPlayer.position) >( aiSenseRadius*aiSenseRadius)&&personalityChosen==2)
			{//if cannot hear player,aggressive tank switches to Check For Flee state
				ChangeState(AIState.CheckForFlee);
			}
			else if (data.currentHealth < data.maxHealth * 0.5f&&personalityChosen==3) {//quiet tank will flee when at half health
				ChangeState(AIState.CheckForFlee);
			}
			if((data.currentHealth<100&&personalityChosen==0)||(CanSee(targetPlayer.position)==false&&personalityChosen==3)) //if quiet tank cannot see target player after stateEnterTime+10 or hp is <100 Check For Flee state switch
			{
				if (Time.time >= stateEnterTime + 10) {
					ChangeState(AIState.CheckForFlee);
				}
			}
			else if (inSights(targetPlayer.position)==true){//if target player is in sights switches to chase and fire state
				ChangeState(AIState.ChaseAndFire);
			}
			timeSinceShot=timeSinceShot+Time.deltaTime;//increments time since last shot
		} 
		else if ( aiState == AIState.ChaseAndFire ) 
		{
		   // Perform Behaviors for Chase And Fire State
		   if (avoidanceStage != 0) {
				DoAvoidance();
			} else 
			{
				motor.Stop();
				motor.rotateTowards(targetPlayer.position,data.rotateSpeed);
				if(personalityChosen==0){//increases speed of explorer tank when it can see player
					float fourthSpeed=data.moveFSpeed/4;//gets a fourth of tank speed
					if(increasedSpeed==false){//if not already equal set to equal
					currentTankData.moveFSpeed=currentTankData.moveFSpeed+fourthSpeed;//sets tank to currentSpeed+1/4
						increasedSpeed=true;
					}
				}
				if(personalityChosen==2&&inSights(targetPlayer.position))//greatly increases speed of aggressive tank(personality number 2) if player is in sights
				{float halfSpeed=data.moveFSpeed/2;//gets a half of tank speed
				    if(increasedSpeed==false){//if not already equal set to equal

					currentTankData.moveFSpeed=currentTankData.moveFSpeed+halfSpeed;
						increasedSpeed=true;
					}
				}
				// Limit our firing rate, so we can only shoot if enough time has passed
				if ((timeSinceShot >=data.fireRate)&&personalityChosen!=2) {
				
					gameObject.GetComponent<shoot>().shootShell(data.shellSize);
					timeSinceShot=0f;//adds time since last frame to timeSinceShot
				}
				else if((timeSinceShot >=data.fireRate)&&personalityChosen==2){
				//ability to shoot a row of bullets determined by random number generator
					float willTripleShot=rngDecision.Next(0,10);
					if(willTripleShot<=5)//single shot if willTripleShot<=5
					{
						if ((timeSinceShot >=data.fireRate))
						{
							gameObject.GetComponent<shoot>().shootShell(data.shellSize);
							timeSinceShot=0f;
						}
				    }
					else if(willTripleShot>5)//shoots row of shells if willTripleShot >5
					{
						if ((timeSinceShot >=data.fireRate)){
							gameObject.GetComponent<shoot>().shootRowOfShells(data.shellSize);
							timeSinceShot=0f;
						}


				    }


				}
              
				timeSinceShot=timeSinceShot+Time.deltaTime;//increments time since last shot

			}
			// Check for Transitions
			if((data.currentHealth<100&&personalityChosen==0)||(CanSee(targetPlayer.position)==false&&personalityChosen==0)) //if explorer tank cannot see target player or hp is <100 Check For Flee state switch
			{

				if (Time.time >= stateEnterTime + 30&&data.currentHealth>100) {
					currentTankData.moveFSpeed=GameManager.instance.mForwardSpeed;//sets speeed back to default
					increasedSpeed=false;
					ChangeState(AIState.CheckForFlee);
				}
				else{
					currentTankData.moveFSpeed=GameManager.instance.mForwardSpeed;//sets speeed back to default
					increasedSpeed=false;
				ChangeState (AIState.CheckForFlee);
				}
			}
			else if(personalityChosen==1)//decide if sporadic tank will flee or continue the chase
			{
				int willFlee=rngDecision.Next(0,11);
				if(willFlee>=6){
					//do nothing
				}
				else if(willFlee<6){//sporadic tank will now flee
					ChangeState (AIState.CheckForFlee);
				}
			}

			else if (Vector3.SqrMagnitude(trans.position-targetPlayer.position) > (aiSenseRadius*aiSenseRadius)&&personalityChosen==2)
			{//if cannot hear target player,aggressive tank switches to Check For Flee state
				currentTankData.moveFSpeed=GameManager.instance.mForwardSpeed;//sets speed back to default
				increasedSpeed=false;
				ChangeState(AIState.CheckForFlee);

			}
			else if (data.currentHealth < data.maxHealth * 0.5f&&personalityChosen==3) {//quiet tank will flee when at half health
				ChangeState(AIState.CheckForFlee);
			}
			 else if (Vector3.SqrMagnitude(trans.position-targetPlayer.position) <= (aiSenseRadius*aiSenseRadius)&&inSights(targetPlayer.position)==false){ 

				if (Time.time >= stateEnterTime +3) {//if tank has not seen the target player for 3 seconds but can still hear them, stops firing continues chasing closer
                   
					    currentTankData.moveFSpeed=GameManager.instance.mForwardSpeed;//sets speed back to default
						increasedSpeed=false;

					ChangeState(AIState.Chase);
				}

			}
			else if(inSights(targetPlayer.position)!=true)
			{
				ChangeState(AIState.Chase);
			}
			timeSinceShot=timeSinceShot+Time.deltaTime;//increments time since last shot
		} else if ( aiState == AIState.Flee ) {
			// Perform Behaviors For Flee State
			if (avoidanceStage != 0) {
				DoAvoidance();
			} else {
				CommenceFlee();
			}
			
			// Check for Transitions
			if (Time.time >= stateEnterTime + 30) {
				ChangeState(AIState.CheckForFlee);
			}
			else if(CanSee(targetPlayer.position)==false&&personalityChosen!=3)//rests when target player cannot be seen
			{
				ChangeState(AIState.Rest);
			}
			else if(Vector3.SqrMagnitude(trans.position-targetPlayer.position) > (aiSenseRadius*aiSenseRadius)&&personalityChosen==3)
			{//rests when target player cannot be heard
				ChangeState(AIState.Rest);
			}
			timeSinceShot=timeSinceShot+Time.deltaTime;//increments time since last shot
		} else if ( aiState == AIState.CheckForFlee ) {
			// Perform Behaviors for Check for Flee State
			CheckForFlee();
			
			// Check for Transitions
			if((data.currentHealth<100&&personalityChosen==0)||(CanSee(targetPlayer.position)==false&&personalityChosen==0)) //if explorer tank cannot see target player or hp is <100 Check For Flee state switch
			{
				if (Time.time >= stateEnterTime + 3) {//if it has been in CheckForFlee state for 3 seconds then goes to flee
					ChangeState(AIState.Flee);
				}
			
			}
			else if(personalityChosen==1)
			{

					ChangeState (AIState.Flee);//already decided to commence flee, continue on to flee for sporadic tank
		    }
			else if (Vector3.SqrMagnitude(trans.position-targetPlayer.position) > (aiSenseRadius*aiSenseRadius)&&personalityChosen==2)
			{//if cannot hear target player,aggressive tank switches to Flee state
				ChangeState(AIState.Flee);
			}
			else if (data.currentHealth < data.maxHealth * 0.5f&&personalityChosen==3) {//quiet tank will flee when at half health
				ChangeState(AIState.Flee);
			}
			 else {

				//Do nothing!
			}
			timeSinceShot=timeSinceShot+Time.deltaTime;//increments time since last shot
		} 
	else if ( aiState == AIState.Rest ) {
			// Perform Behaviors
			DoRest();
			
			// Check for Transitions
			if (Vector3.SqrMagnitude(trans.position-playerLoc.position) <= (aiSenseRadius*aiSenseRadius)&&personalityChosen!=2){//if can hear player one flees further as long as it is not an agressive tank(personality type 2)
				ChangeState(AIState.Flee);
			}
			else if (Vector3.SqrMagnitude(trans.position-playerTwoLoc.position) <= (aiSenseRadius*aiSenseRadius)&&personalityChosen!=2){//if can hear player two flees further as long as it is not an agressive tank(personality type 2)
				ChangeState(AIState.Flee);
			}
			else if(Vector3.SqrMagnitude(trans.position-playerLoc.position)<=(aiSenseRadius*aiSenseRadius)&&personalityChosen==2){
				ChangeState (AIState.Chase);
			}//if agressive tank(personality type 2) can hear player one they will chase them
			else if(Vector3.SqrMagnitude(trans.position-playerTwoLoc.position)<=(aiSenseRadius*aiSenseRadius)&&personalityChosen==2){
				ChangeState (AIState.Chase);
			}//if agressive tank(personality type 2) can hear player two they will chase them
			else if (data.currentHealth >= data.maxHealth) {
				ChangeState(AIState.Patrol);
			}
			timeSinceShot=timeSinceShot+Time.deltaTime;//increments time since last shot
		}

	}


	void DoChase () {//chases target in chase state
		motor.rotateTowards (targetPlayer.position, data.rotateSpeed);
		if (CanMove (data.moveFSpeed)) {

			motor.Move (data.moveFSpeed);
		}
		else{
			avoidanceStage=1;
		}
	}
		void DoAvoidance () {//avoid obstacles
		if (avoidanceStage == 1) {
			// Rotate left
			motor.Stop ();
			motor.Rotate (-data.rotateSpeed);
			motor.Stop ();
				
			// If I can now move forward, move to stage 2!
			if (CanMove (data.moveFSpeed)) {
				avoidanceStage = 2;
					
				// Set the number of seconds we will stay in Stage2
				exitTime = avoidanceTime;
			}
			// Otherwise, we'll do this again next turn!
		} else if (avoidanceStage == 2) {
			// if we can move forward, do so
			motor.Stop ();
			if (CanMove (data.moveFSpeed)) {
				// Subtract from our timer and move
				exitTime -= Time.deltaTime;
				motor.Move (data.moveFSpeed);
					
				// If we have moved long enough, return to chase mode
				if (exitTime <= 0) {
					avoidanceStage = 0;
				}                     
			} 
			else {
				motor.Stop ();
				// Otherwise, we can't move forward, so back to stage 1
				avoidanceStage = 1;
			}     
		}
	}
		public void CheckForFlee() {//does checkForFlee things if neccessary
		increasedSpeed = false;
		  
		}
		
		public void DoRest() {//rests tank and heals them until they are full or player comes near
		data.currentHealth += ((restingHealRate * Time.deltaTime)*2);//increases health every 2 seconds that tank is resting
		// never goes over max health
		data.currentHealth = Mathf.Min (data.currentHealth, data.maxHealth);
		}
	void CommenceFlee(){//moves tank away from player
		//vector from ai enemy tank to player is playerLoc position minus enemy tank position.
		Vector3 vectorToPlayer = targetPlayer.position - trans.position;

	//multiply by -1 to get a vector AWAY from target
		Vector3 vectorAwayFromPlayer = -1 * vectorToPlayer;
		
		//normalize that vector to give it a magnitude of 1
		vectorAwayFromPlayer.Normalize ();
		
		//multiply normalized vector by fleeing distance to make a vector of that length to increase the distance the enemy tank will flee
		vectorAwayFromPlayer *= fleeingDistance;
		
		// find the position in space for enemy tank to move to by adding vectorAway from our player to our AI's position.
		//     This gives us a point that is "that vector away" from our current position.
		Vector3 fleePosition = vectorAwayFromPlayer + trans.position;
		motor.rotateTowards (fleePosition, data.rotateSpeed);
		motor.Move (data.moveFSpeed);
	}
	public void DoPatrol()//does tanks patrol
	{  //chooses patrol fuction to run based on tank personality
			if (personalityChosen == 0 || personalityChosen == 1) {
				loopPatrol();
		   }
		else if (personalityChosen==2){
				pingPongTank();
			}
		else{
				stopHere();
			}


	}
		public void stopHere()//stops tank at last waypoint
	{	
		Vector3 target = patrolPoints [currentWaypoint].position;
		Vector3 targetLevel = new Vector3 (target.x, trans.position.y, target.z);
		if (motor.rotateTowards (targetLevel, data.rotateSpeed)==true) {//use rotateTowards to rotate to target waypoint


		} else if(CanMove(data.moveFSpeed)==true){
			// Move forward
			motor.Move (data.moveFSpeed);
		
		}
	    if(CanMove (data.moveFSpeed)==false) {//if player cannot move will enter avoidance stage one
			// Enter obstacle avoidance stage 1
			motor.Stop ();
			avoidanceStage = 1;
		}
		if (Vector3.SqrMagnitude (patrolPoints [currentWaypoint].position - trans.position) <= (closeEnough * closeEnough)) {

			if (currentWaypoint < patrolPoints.Length - 1) {
				currentWaypoint++;
			
			}
		}
		if (Vector3.SqrMagnitude (patrolPoints [currentWaypoint].position - trans.position) <= (closeEnough * closeEnough)&&currentWaypoint==3) {//stop moving when last waypoint has been reached
			motor.Stop ();
		}


		}
	public void loopPatrol()//used to loop tank through waypoints
	{
		if (motor.rotateTowards (patrolPoints [currentWaypoint].position, data.rotateSpeed)==true) {//use rotateTowards to rotate to target waypoint
			//motor.Stop ();
		} else if(CanMove(data.moveFSpeed)==true){
			// Move forward
			motor.Move (data.moveFSpeed);
		}
		else {//if tank cannot move will enter avoidance stage one
			motor.Stop();
			// Enter obstacle avoidance stage 1
			avoidanceStage = 1;
		}
		if (Vector3.SqrMagnitude (patrolPoints [currentWaypoint].position - trans.position) <= (closeEnough * closeEnough)) {
			// Advance to the next waypoint, if we are still in range
			if (currentWaypoint < patrolPoints.Length - 1) {
				currentWaypoint++;
			} else {
				currentWaypoint = 0;
			}
		}
	}
	public void pingPongTank()///ping pongs tank between closest waypoints
	{	if (motor.rotateTowards (patrolPoints [currentWaypoint].position, data.rotateSpeed)==true) {//use rotateTowards to rotate to target waypoint
			motor.Stop();
		} else if(CanMove(data.moveFSpeed)==true){
			// Move forward
			motor.Move (data.moveFSpeed);
		}
		else {//if player cannot move will enter avoidacne stage one
			motor.Stop();
			// Enter obstacle avoidance stage 1
			avoidanceStage = 1;
		}
		if (Vector3.SqrMagnitude (patrolPoints [currentWaypoint].position - trans.position) <= (closeEnough * closeEnough)) {
			if (isPatrolForward) {
				// Advance to the next waypoint, if we are still in range
				if (currentWaypoint < patrolPoints.Length - 1) {
					currentWaypoint++;
				} else {
					//Otherwise reverse direction and decrement our current waypoint
					isPatrolForward = false;
					currentWaypoint--;
				}
			} else {
				// Advance to the next waypoint, if we are still in range
				if (currentWaypoint > 0) {
					currentWaypoint--;
				} else {
					//Otherwise reverse direction and increment our current waypoint
					isPatrolForward = true;
					currentWaypoint++;
				}
			}
		}
	}
	bool CanMove ( float speed ) {//checks if tank can move
		// Cast a ray forward in the distance that we sent in
		// If our raycast hit something
		RaycastHit hit;
		if (Physics.Raycast (trans.position, trans.forward, out hit, speed)) {


			if(!hit.collider.CompareTag ("playerOne")){//if raycast hits player one will not move

				
				return false;
			}
			if(!hit.collider.CompareTag ("playerTwo")){//if raycast hits player two will not move
				return false;
			}

		}

	

	
		return true;
	}
	public bool CanSee ( Vector3 targetPosition )//used to detect if tank can see player
	{
		// Find the vector from the agent to the target
		// We do this by subtracting "destination minus origin", so that "origin plus vector equals destination."
		Vector3 agentToTargetVector = targetPosition - transform.position;
		
		// Find the angle between the direction our agent is facing (forward in local space) and the vector to the target.
		float angleToTarget = Vector3.Angle (agentToTargetVector, transform.forward);
		
		// if that angle is less than our field of view
		if ( angleToTarget < FOVsight )
		{
			// Create a variable to hold a ray from our position to the target
			Ray rayToTarget = new Ray();
			
			// Set the origin of the ray to our position, and the direction to the vector to the target
			rayToTarget.origin = transform.position;
			rayToTarget.direction = agentToTargetVector;
			
			// Create a variable to hold information about anything the ray collides with
			RaycastHit hitInfo;
			
			// Cast our ray for infinity in the direciton of our ray.
			//    -- If we hit something...
			if (Physics.Raycast (rayToTarget, out hitInfo, Mathf.Infinity)) {
				// ... and that something is our target 
				if (hitInfo.collider.transform.position == playerLoc.position) {
					// return true if player one target can be seen
					targetPlayer=playerLoc;//set targetPlayer to playerLoc
					return true;
				}
				else if(hitInfo.collider.transform.position==playerTwoLoc.position){
					//return true if player two target can be seen
					targetPlayer=playerTwoLoc;//set targetPlayer to playerTwoLoc
					return true;
				}
			}
		}
		//returns false is nothing is hit or what is hit is not the target
		return false; 
	}
public bool inSights(Vector3 targetPosition)//used to determine if player tank is close enough to enemy tank to be considered in sights
	{// Find the vector from the agent to the target
		// We do this by subtracting "destination minus origin", so that "origin plus vector equals destination."
		Vector3 agentToTargetVector = targetPosition - transform.position;
		
		// Find the angle between the direction our agent is facing (forward in local space) and the vector to the target.
		float angleToTarget = Vector3.Angle (agentToTargetVector, transform.forward);
		
		// if that angle is less than our player in sights field of view
		if ( angleToTarget < playerInSightsFOV )
		{
			// Create a variable to hold a ray from our position to the target
			Ray rayToTarget = new Ray();
			
			// Set the origin of the ray to our position, and the direction to the vector to the target
			rayToTarget.origin = transform.position;
			rayToTarget.direction = agentToTargetVector;
			
			// Create a variable to hold information about anything the ray collides with
			RaycastHit hitInfo;
			
			// Cast our ray for infinity in the direciton of our ray.
			//    -- If we hit something...
			if (Physics.Raycast (rayToTarget, out hitInfo, Mathf.Infinity)) {
				// ... and that something is our target 
				if (hitInfo.collider.transform.position == playerLoc.position) {
					// return true if player one target is hit
					targetPlayer=playerLoc;
					return true;
				}
				else if(hitInfo.collider.transform.position==playerTwoLoc.position){
					//return true if player two target can be seen
					targetPlayer=playerTwoLoc;//set targetPlayer to playerTwoLoc
					return true;
				}
			}
		}
		// return false if we hit nothing or what was hit was not target

		return false; 
	}
	public void hit(string shooter)//used to subtract health
	{
		data.currentHealth=data.currentHealth-data.damageAmount;
		AudioSource.PlayClipAtPoint(impactSound,currentEnemyTank.transform.position,GameManager.instance.sfxVolume);
		if (data.currentHealth <= 0) {//destroys tank

			if(shooter!="shell"){
				GameObject player=GameObject.Find (shooter);
			    if(player.GetComponent<TankData>()!=null){
				player.GetComponent<TankData>().playerScore = player.GetComponent<TankData>().playerScore + gameObject.GetComponent<TankData>().pointValue;//adds point value of tank to player score
				}
					AudioSource.PlayClipAtPoint(deathSound,currentEnemyTank.transform.position,GameManager.instance.sfxVolume);
				destroyTank ();
				GameManager.instance.enemyRespawn(gameObject);
				
			}


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

		gameObject.GetComponent<CharacterController> ().enabled = false;
	}
	public void tankRespawn(){//used to reactivate a tank/respawn it
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
		gameObject.SetActive (true);
		gameObject.GetComponent<CharacterController> ().enabled = true;
		gameObject.GetComponent<SphereCollider> ().enabled = true;

		data.currentHealth = data.maxHealth;//sets tanks hp back to max upon respawning
	}

	public void ChangeState ( AIState newState ) {  // Change our state
		aiState = newState;
		
		// save the time we changed states
		stateEnterTime = Time.time;
		}
	}

