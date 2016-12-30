using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GameManager : MonoBehaviour {
	public static GameManager instance;
	//variables needed to allow designers to easily find and set important variables for testing via the GameManager
	private TankData playerData;
	private shoot shellData;
	private GameObject enemyTankPrefab;//used to get enemyTank prefab and spawn copies
	private GameObject playerTankOnePrefab;//used to get player tank prefabs and spawn copies
	private GameObject playerTankTwoPrefab;
	private GameObject mapGen;//used to get mapGenerator class
	private mapGenerator generate;//used to get mapGenerator class
    private GameObject pauseMenu;//used to get options menu and disable/enable it
	private GameObject options;//used to get options menu and disable/enable it
	private GameObject failure;//used to get game over screen and disable/enable it
	private GameObject playerOneInfo;//used to get lives/score ui for player one
	private GameObject playerTwoInfo;//used to get lives/score ui for player two
	private GameObject mainGameSound;//used to get main game sound object and make sure it does not play on menus
	private GameObject inputName;//used to get input field for inputing names for the high score list
	private GameObject isHighText;//used to get high score acheied text
	private GameObject save;//used to get gameobject for saving
	private Camera p1Camera;//used to get player one camera and adjust ViewportRect variables as needed
	private GameObject failCam;//used to get camera on gameoverScreen
	[Header("Tanks,Personalities,and Waypoints")]
	public GameObject[]players;//holds player gameobjects for easy access of other classes
	public GameObject[]enemies;//hold enemy gameObjects for easy access of other classes
	public enum tankPersonality{Explorer, Sporadic, Aggressive, Quiet};//decides behavior of tank based on their personality
	public tankPersonality personality1;//select a personality for tank1
	public tankPersonality personality2;//used for tank 2
	public tankPersonality personality3;//used for tank 3
	public tankPersonality personality4;//used for tank 4
	public Transform[] patrolPoints;//holds waypoints for patroling
	[Header("Tank movement/Shooting Data")]
	public float mForwardSpeed=5; // in meters per second
	public float mBackwardSpeed = 5;
	public float rotationSpeed=180; //in degrees per second
	public int fRate=5;
	public float sForce=20;
	public int shellDamageAmount=10;
	[Header("Tank Survival Data")]
	public int hp=500;//health of tank, when it reaches <=0 a life is lost
	public int pointsWorth=5;
	public int lives;//used to set lives of tank
	[Header("Tank Senses And AI")]
	public float closeEnough=3.0f;//determines if tank is close enough to target
	public float hearingRadius=8.0f;//distance at which tank can hear
	public float seeingDistance=4.0f;//distance at which tank can see
	public float playerInSightDistance=1.0f;//distance at which player is in tank sights
	public float fleeingDistance=8.0f;//distance to which an enemy tank will attempt to flee in flee state
	public float avoidanceTime = 2.0f;//time that tank will attempt to avoid obstacles before moving forward
	public float restingTime=10.0f;//time that tank will rest to heal
	public float hpHealOverTick=10;//hp that tank will heal upon tick
	public Room[,] mapGrid;
	[Header("Map Variables")]
	public int mapSeed;//seed for map generator
	public int mapRows;//rows in map generated
	public int mapColumns;//columns in map generated
	public bool useMapOfDay;//generates map of day if checked
	public bool useRandomMap;//generates a random map
	public GameObject[] mapTiles;//map tiles to be used for generating map
	[Header("Timers")]
	public int timeOutOfShell=60;//period in which existing shells are destroyed
	public float powerupSpawnDelay;//time between powerup spawns
	[Header("Volumes And Game Mode")]
	public float musicVolume;//used to set musicVolume
	public float sfxVolume;//used to set sfx volume
	public enum playMode{singlePlayer, multiplayer};
	public playMode selectMode;//used to select a playmode
	void Awake()
	{
		if (instance == null) {//checks if instance of GameManager already exists,and creates one if not
			instance = this;
		}
		else {
			Debug.LogError("ERROR: GameManager already exists.");//prints error to console if instance exists
			Destroy(gameObject);
		}
		mapGen = GameObject.Find ("mapOrgin");
        pauseMenu = GameObject.Find("pauseScreen");
		options = GameObject.Find ("optionsMenu");
		failure = GameObject.Find ("gameOverScreen");
		playerOneInfo = GameObject.Find ("livesScoreP1");
		playerTwoInfo = GameObject.Find ("livesScoreP2");
		mainGameSound = GameObject.Find ("mainGameSound");
		inputName = GameObject.FindWithTag ("nameInput");
		isHighText = GameObject.FindWithTag ("isHigh");
		failCam=GameObject.FindGameObjectWithTag("failCam");
		save=GameObject.Find ("SaveManager");

	
	}
	// Use this for initialization
	void Start () {//disable all components that should not be visable at start
		if (pointsWorth < 0) {//makes sure desinger can not set pointsWorth to less than 0
			pointsWorth = 0;
		}
		options.SetActive (false);
        pauseMenu.SetActive(false);
		Transform p1InfoCan = playerOneInfo.transform.Find ("Canvas");
		Transform p2InfoCan = playerTwoInfo.transform.Find ("Canvas");
		p1InfoCan.GetComponentInChildren<Text>().enabled=false;
		p2InfoCan.GetComponentInChildren<Text>().enabled=false;
		disableFailScreen ();
		mainGameSound.SetActive (false);
		enemyTankPrefab = GameObject.Find("prefabs").GetComponent<enemyPrefabStore>().enemyTankPref;//gets enemyTank prefab
		playerTankOnePrefab = GameObject.Find ("prefabs").GetComponent<enemyPrefabStore> ().playerTankPref;//gets player tank prefab
		playerTankTwoPrefab = GameObject.Find ("prefabs").GetComponent<enemyPrefabStore> ().playerTank2Pref;//gets player 2 tank prefab
	    save.GetComponent<SaveManager> ().Load ();//loads save game
	}
	
	// Update is called once per frame
	void Update () {

        if (GameObject.Find("p1Text").GetComponent<Text>().enabled==true|| GameObject.Find("p2Text").GetComponent<Text>().enabled == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))//allow user to pause game
            {
                pauseMenu.SetActive(true);//disabled all scripts neccessary for pausing of game
                foreach(GameObject player in players)
                {
                    player.GetComponent<InputController>().enabled = false;
                    player.GetComponent <tankMotor>().enabled = false;
                    player.GetComponent<TankData>().enabled = false;
                    player.GetComponent<PowerUpController>().enabled = false;
                    player.GetComponent<shoot>().enabled = false;

                    
                }
                foreach (GameObject enemy in enemies)
                {
                    enemy.GetComponent<AIController>().enabled = false;
                    enemy.GetComponent<tankMotor>().enabled = false;
                    enemy.GetComponent<TankData>().enabled = false;
                    enemy.GetComponent<PowerUpController>().enabled = false;
                    enemy.GetComponent<shoot>().enabled = false;


                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))//allow user to quit game
            {
                Application.Quit();
            }
        }
	}
	public void disableFailScreen()//disables all visable game over screen components
	{
		Transform endCanvas = failure.transform.Find ("Canvas");
		endCanvas.transform.Find ("scoreTitle").GetComponent<Text> ().enabled=false;
		endCanvas.transform.Find ("scoreText").GetComponent<Text> ().enabled = false;
		endCanvas.transform.Find ("scoreText").GetComponent<scoreListMod>().enabled = false;
		endCanvas.transform.Find ("scoreText").GetComponentInChildren<Text> ().enabled = false;
		endCanvas.transform.Find ("gameOverText").GetComponent<Text> ().enabled = false;
		failCam.SetActive (false);
		isHighText.GetComponent<Text> ().enabled = false;
		endCanvas.transform.Find ("finalScoreText").GetComponent<Text> ().enabled = false;
		endCanvas.transform.Find ("restartButton").GetComponent<Image> ().enabled = false;
		endCanvas.transform.Find ("restartButton").GetComponent<Button> ().enabled = false;
		endCanvas.transform.Find ("restartButton").GetComponentInChildren<Text> ().enabled = false;
		endCanvas.transform.Find ("restartButton").GetComponent<Image> ().enabled = false;
		endCanvas.transform.Find ("menuReturnButton").GetComponent<Image> ().enabled = false;
		endCanvas.transform.Find ("menuReturnButton").GetComponent<Button> ().enabled = false;
		endCanvas.transform.Find ("menuReturnButton").GetComponentInChildren<Text> ().enabled = false;
		inputName.GetComponent<Image> ().enabled = false;
		inputName.GetComponent<InputField> ().enabled = false;
		inputName.transform.Find ("Placeholder").GetComponent<Text> ().enabled = false;
		inputName.transform.Find ("Text").GetComponent<Text> ().enabled = false;
	}
	public void enableFailScreen()//enables all visable game over screen components
	{Transform endCanvas = failure.transform.Find ("Canvas");
		endCanvas.transform.Find ("scoreTitle").GetComponent<Text> ().enabled=true;
		endCanvas.transform.Find ("scoreText").GetComponent<Text> ().enabled = true;
		endCanvas.transform.Find ("gameOverText").GetComponent<Text> ().enabled = true;
		failCam.SetActive (true);
		endCanvas.transform.Find ("finalScoreText").GetComponent<Text> ().enabled = true;
		endCanvas.transform.Find ("restartButton").GetComponent<Image> ().enabled = true;
		endCanvas.transform.Find ("restartButton").GetComponent<Button> ().enabled = true;
		endCanvas.transform.Find ("restartButton").GetComponentInChildren<Text> ().enabled = true;
		endCanvas.transform.Find ("restartButton").GetComponent<Image> ().enabled = true;
		endCanvas.transform.Find ("menuReturnButton").GetComponent<Image> ().enabled = true;
		endCanvas.transform.Find ("menuReturnButton").GetComponent<Button> ().enabled = true;
		endCanvas.transform.Find ("menuReturnButton").GetComponentInChildren<Text> ().enabled = true;
		endCanvas.transform.Find ("scoreText").GetComponentInChildren<Text> ().enabled = true;
		//endCanvas.transform.Find ("scoreText").GetComponent<scoreListMod>().enabled = false;
	}
	public void enableInputField()//used to enable input field and highscore text
	{
		Transform endCanvas = failure.transform.Find ("Canvas");
		isHighText.GetComponent<Text> ().enabled = true;
		endCanvas.transform.Find ("scoreText").GetComponentInChildren<Text> ().enabled =true;
		endCanvas.transform.Find ("scoreText").GetComponent<scoreListMod> ().enabled = true;
		inputName.GetComponent<Image> ().enabled = true;
		inputName.GetComponent<InputField> ().enabled = true;
		inputName.transform.Find ("Placeholder").GetComponent<Text> ().enabled =true;
		inputName.transform.Find ("Text").GetComponent<Text> ().enabled = true;
	}
	public void disableInputField()//used to disable input field and highscore text without disabling other components
	{
		Transform endCanvas = failure.transform.Find ("Canvas");
		isHighText.GetComponent<Text> ().enabled = false;
		endCanvas.transform.Find ("scoreText").GetComponent<scoreListMod> ().enabled = false;
	   // endCanvas.transform.Find ("scoreText").GetComponentInChildren<Text> ().enabled = false;
		inputName.GetComponent<Image> ().enabled = false;
		inputName.GetComponent<InputField> ().enabled = false;
		inputName.transform.Find ("Placeholder").GetComponent<Text> ().enabled =false;
		inputName.transform.Find ("Text").GetComponent<Text> ().enabled =false;
	
	}
	public void playersInfo()//enables player info
	{
		Transform p1InfoCan = playerOneInfo.transform.Find ("Canvas");
		Transform p2InfoCan = playerTwoInfo.transform.Find ("Canvas");
		p1InfoCan.GetComponentInChildren<Text>().enabled=true;

		
	
		if (GameManager.instance.selectMode == GameManager.playMode.multiplayer) 
		{
			p2InfoCan.GetComponentInChildren<Text>().enabled=true;
		}
	}
	public void newGame()
	{//sets up new game
		if (generate == null) {
			generate = mapGen.GetComponent<mapGenerator> ();
		}
		GameObject[] eClones=GameObject.FindGameObjectsWithTag("enemyTank");
		GameObject[] pClones=GameObject.FindGameObjectsWithTag("playerOne");
		GameObject[] p2Clones=GameObject.FindGameObjectsWithTag("playerTwo");
		GameObject[] hpSpawns = GameObject.FindGameObjectsWithTag ("hp");
		GameObject[] increasedFire = GameObject.FindGameObjectsWithTag ("fireIncrease");
		GameObject[] increasedSpeed = GameObject.FindGameObjectsWithTag ("speedP");
		GameObject[] increasedBulletSize = GameObject.FindGameObjectsWithTag ("increasedSSize");
		foreach(GameObject clone in eClones){//destroy all enemy clones
			Destroy(clone);
		}
		foreach(GameObject clone in pClones){//destroy all player tanks
			Destroy(clone);
		}
		foreach(GameObject clone in p2Clones){//destroy all player tanks
			Destroy(clone);
		}
		foreach(GameObject clone in hpSpawns){//destroy all hpRengerator powerups 
			Destroy(clone);
		}
		foreach(GameObject clone in increasedFire){//destroy all increasedFire powerups
			Destroy(clone);
		}
		foreach(GameObject clone in increasedSpeed){//destroy all milkForSpeed powerups
			Destroy(clone);
		}
		foreach(GameObject clone in increasedBulletSize){//destroy all shellIncreaser powerups
			Destroy(clone);
		}

		generate.GenerateGrid();
		mainGameSound.SetActive (true);
		int selectRow = UnityEngine.Random.Range (0, mapRows);//obtain random row and column
		int selectColumn = UnityEngine.Random.Range (0, mapColumns);
		Room spawnRoom=mapGrid[selectColumn,selectRow];//get room corresponding to random row and column,use to spawn player
		Transform spawnerLoc=spawnRoom.transform.Find ("playerSpawn");//used to get playerSpawn located in specific room
		if (selectMode == playMode.singlePlayer) {//spawn one player for single if(players[0]!=Instantiate (playerTankOnePrefab, spawnerLoc.transform.position, Quaternion.identity)as GameObject){
			   players[0]=Instantiate (playerTankOnePrefab, spawnerLoc.transform.position, Quaternion.identity)as GameObject;//instantiate player in random location
				p1Camera=players[0].GetComponentInChildren<Camera>();
			p1Camera.rect=new Rect(0,0,1,1);
		} else {//spawn two players for multiplayer
			players[0]=Instantiate (playerTankOnePrefab, spawnerLoc.transform.position, Quaternion.identity)as GameObject;//instantiate player in random location
			selectRow=UnityEngine.Random.Range (0,mapRows);
			selectColumn=UnityEngine.Random.Range (0,mapColumns);
			Room spawnP2Room=mapGrid[selectColumn,selectRow];//generate new random numbers for row and column to ensure tanks are not always placed on top of each other
			while(spawnP2Room==spawnRoom)//make sure it keeps generating numbers until position is different
			{
				selectRow=UnityEngine.Random.Range (0,mapRows);
				selectColumn=UnityEngine.Random.Range (0,mapColumns);
				spawnP2Room=mapGrid[selectColumn,selectRow];

			}
			spawnerLoc=spawnP2Room.transform.Find ("playerSpawn");
			players[1]=Instantiate (playerTankTwoPrefab, spawnerLoc.transform.position, Quaternion.identity)as GameObject;//instantiate player in random location
			players[1].GetComponent<InputController>().playerInput=InputController.InputScheme.arrowKeys;//sets player two input controller scheme
		}

		int newEnemyRow;//row of room to spawn enemy in
		int newEnemyColumn;//column of room to spawn enemy in
		Room enemySpawnRoom;//room corresponding to random row and column, use to spawn enemies
		Transform enemySpawnLoc;//used to get enemySpawn located in specific room
		for (int n=0; n<4; n++) 
		{//loops until 4 enemy tanks are spawned
			newEnemyRow = UnityEngine.Random.Range (0, mapRows);
			newEnemyColumn = UnityEngine.Random.Range (0, mapColumns);
			enemySpawnRoom = mapGrid [newEnemyColumn, newEnemyRow];
			enemySpawnLoc = enemySpawnRoom.transform.Find ("enemySpawn");
			enemies[n] = Instantiate (enemyTankPrefab, enemySpawnLoc.transform.position, Quaternion.identity)as GameObject;//stores enemy temporaily in tempEnemy before giving it a meaninful name
			if (n == 0) 
			{//names first tank spawned enemyTank
				enemies [n].name="enemyTank";
			} else 
			{//names all other enemyTanks spawned enemyTank plus n ex:enemyTank 2
				enemies [n].name="enemyTank " + n;
			}

			//loops up to 4 and sets all starting patrolPoints for enemyTanks
				for(int w=0;w<4;w++)
				{
					enemies [n].GetComponent<TankData> ().patrolPoints [w] = enemySpawnRoom.transform.Find ("waypoint" + (w + 1));
				}

		}

			

	}
	public void reSpawn()//respawns player upon death
	{
        
		if (players [0].GetComponent<TankData> ().currentHealth <= 0 && players [0].GetComponent<TankData> ().lives > 0) {//if player dies instantiate them in random section
			int selectRow = UnityEngine.Random.Range (0, mapRows);
			int selectColumn = UnityEngine.Random.Range (0, mapColumns);
			Room spawnRoom = mapGrid [selectColumn, selectRow];
			Transform spawnerLoc = spawnRoom.transform.Find ("playerSpawn");//used to get playerSpawn located in specific room
			players [0].GetComponent<InputController> ().tankRespawn ();
			players [0].transform.position = spawnerLoc.transform.position;
		}
		if (players [1].GetComponent<TankData> ().currentHealth <= 0 && players [1].GetComponent<TankData> ().lives > 0) {//if player dies instantiate them in random section
			int selectRow = UnityEngine.Random.Range (0, mapRows);
			int selectColumn = UnityEngine.Random.Range (0, mapColumns);
			Room spawnRoom = mapGrid [selectColumn, selectRow];
			Transform spawnerLoc = spawnRoom.transform.Find ("playerSpawn");//used to get playerSpawn located in specific room
			players [1].GetComponent<InputController> ().tankRespawn ();
			players [1].transform.position = spawnerLoc.transform.position;
		}
		if (selectMode == playMode.singlePlayer && players [0].GetComponent<TankData> ().currentHealth <= 0 && players [0].GetComponent<TankData> ().lives <= 0) {//if single player mode and player has 0 hp make gameOverScreen active upon reaching lives <=0
			mainGameSound.SetActive (false);
			Transform p1InfoCan = playerOneInfo.transform.Find ("Canvas");
			p1InfoCan.GetComponentInChildren<Text> ().enabled = false;
	
			for (int i=0; i<4; i++) {
				if (enemies [i].activeSelf == false && enemies [i] != null) {
					enemies [1].SetActive (true);
				}
			}
			if (players [0].activeSelf == false && players [0] != null) {
				players [0].SetActive (true);
			}

			for (int w=0; w<4; w++) {//disables aiControllers until they are needed again
				enemies [w].GetComponent<AIController> ().enabled = false;
			}
			generate.clear ();
			mapGrid = new Room[mapRows, mapColumns];
			enableFailScreen ();
			Transform endCanvas = failure.transform.Find ("Canvas");
			endCanvas.transform.Find ("scoreText").GetComponent<scoreListMod> ().enabled = true;

		}
	
	

		else if (players [0].GetComponent<TankData> ().currentHealth <=0 && players [0].GetComponent<TankData> ().lives <=0 &&
			players [1].GetComponent<TankData> ().currentHealth <=0 && players [1].GetComponent<TankData> ().lives <=0&&selectMode==playMode.multiplayer) {//if both players have 0 hp and have less than or equal to 0 lives make gameOverScreen active
			Transform p1InfoCan = playerOneInfo.transform.Find ("Canvas");
			Transform p2InfoCan = playerTwoInfo.transform.Find ("Canvas");
			p1InfoCan.GetComponentInChildren<Text>().enabled=false;
			p2InfoCan.GetComponentInChildren<Text>().enabled=false;
			mainGameSound.SetActive (false);
	        if(players[0].activeSelf==false)
			{
				players[0].SetActive(true);
			}
			if(players[1].activeSelf==false)
			{
				players[1].SetActive(true);
			}
			for(int i=0;i<4;i++)
			{
				if (enemies[i].activeSelf==false&&enemies!=null)
				{
					enemies[1].SetActive(true);
				}
			}
		
			for(int w=0;w<4;w++)//disables aiControllers until they are needed again
			{
				enemies [w].GetComponent<AIController> ().enabled=false;
			}

			generate.clear ();
			mapGrid=new Room[mapRows,mapColumns];
		
			enableFailScreen();
			Transform endCanvas = failure.transform.Find ("Canvas");
			endCanvas.transform.Find ("scoreText").GetComponent<scoreListMod> ().enabled = true;
		}


	}
	public void enemyRespawn(GameObject enemy)
	{
		int selectRow=UnityEngine.Random.Range (0,mapRows);
		int selectColumn=UnityEngine.Random.Range (0,mapColumns);
		Room spawnRoom = mapGrid [selectColumn, selectRow];
		Transform spawnerLoc = spawnRoom.transform.Find ("enemySpawn");//used to get enemySpawn located in specific room
		enemy.GetComponent<AIController> ().tankRespawn ();
		enemy.transform.position=spawnerLoc.transform.position;
		enemy.GetComponent<TankData> ().currentHealth = enemy.GetComponent<TankData> ().maxHealth;//sets hp back to max upon respawning

	}

}
