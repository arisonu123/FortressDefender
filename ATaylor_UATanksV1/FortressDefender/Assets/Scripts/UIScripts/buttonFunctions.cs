using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;//needed to use Regex to get numbers from strings
using UnityEngine.UI;
public class buttonFunctions : MonoBehaviour {
	private GameObject mainMenu;
	private GameObject optionsMenu;
    private GameObject pauseMenu;
	private GameObject gameOverScreen;
	private GameObject p1Info;
	private GameObject p2Info;
	private GameObject buttonDownContainer;
	private GameObject buttonUpContainer;
	private GameObject isHighText;
	private GameObject scoreListInput;
	private GameObject scoreListText;
	private string scoreFormat;
	private Text scoreList;
	private Text whoIsHigh;
	private Text playerName;
	private GameObject save;//used to get object to save game
	private scoreListMod modifier;
	public AudioClip buttonDown;
	public AudioClip buttonUp;
	public AudioSource buttonPressed;
	public AudioSource buttonReleased;
	// Use this for initialization
    void Awake()
    {
        pauseMenu = GameObject.Find("pauseScreen");//stores pause menu ui object for purpose of disabling/enabling
    }
	void Start () {
		mainMenu = GameObject.Find ("mainMenuUI");//stores main menu ui object for purpose of disabling/enabling
		optionsMenu = GameObject.Find ("optionsMenu");
		buttonDownContainer = GameObject.Find ("buttonDownSound");
		buttonUpContainer = GameObject.Find ("buttonUpSound");
		isHighText = GameObject.FindWithTag ("isHigh");
		scoreListInput = GameObject.FindWithTag ("nameInput");
		scoreListText=GameObject.FindWithTag ("scoreList");
		save=GameObject.Find ("SaveManager");
	    modifier=scoreListText.GetComponentInParent<scoreListMod> ();
		scoreList = scoreListText.GetComponent<Text>();
		whoIsHigh = isHighText.GetComponent<Text>();
		playerName = scoreListInput.transform.Find ("Text").GetComponent<Text>();
		buttonPressed = buttonDownContainer.GetComponent<AudioSource> ();
		buttonReleased = buttonUpContainer.GetComponent<AudioSource> ();
		buttonDown = audioList.instance.buttonSounds;
		buttonUp = audioList.instance.buttonSounds;
        

	}
	
	// Update is called once per frame
	void Update () {

	}
	public void startGame()//disables main menu and starts a new game
	{
		mainMenu.SetActive (false);
		GameManager.instance.disableFailScreen ();
		GameManager.instance.playersInfo ();
		scoreListText.GetComponent<scoreListMod> ().setRunThru ();
		scoreListText.GetComponentInParent<scoreListMod> ().scoreState=scoreListMod.scoreDecision.checkScores;
		save.GetComponent<SaveManager> ().Save ();//saves game upon returning to menu
		GameManager.instance.newGame();

	}
	public void optionsEnable()//enables options menu and disables mainMenu
	{
		optionsMenu.SetActive(true);
		mainMenu.SetActive (false);
	
	}
    public void pauseEnable()//enables pause menu
    {
        pauseMenu.SetActive(true);
    }
    public void resumeGame()//resume game
    {
        foreach (GameObject player in GameManager.instance.players)
        {
            player.GetComponent<InputController>().enabled = true;
            player.GetComponent<tankMotor>().enabled = true;
            player.GetComponent<TankData>().enabled = true;
            player.GetComponent<PowerUpController>().enabled = true;
            player.GetComponent<shoot>().enabled = true;


        }
        foreach (GameObject enemy in GameManager.instance.enemies)
        {
            enemy.GetComponent<AIController>().enabled = true;
            enemy.GetComponent<tankMotor>().enabled = true;
            enemy.GetComponent<TankData>().enabled = true;
            enemy.GetComponent<PowerUpController>().enabled = true;
            enemy.GetComponent<shoot>().enabled = true;


        }
        pauseMenu.SetActive(false);
        
    }
    public void quitGame()//quits game
    {
      foreach(GameObject player in GameManager.instance.players)
        {
            player.GetComponent<TankData>().currentHealth = 0;
            player.GetComponent<TankData>().lives = 0;
            GameManager.instance.reSpawn();
            pauseMenu.SetActive(false);
        }
    }
	public void returnMenu()//returns player to main menu from options menu
	{
		save.GetComponent<SaveManager> ().Save ();//saves game upon returning to menu
		optionsMenu.SetActive (false);
		GameManager.instance.disableFailScreen ();
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
		mainMenu.SetActive (true);
	}
	public void musicUp()//increases music volume
	{
		if (GameManager.instance.musicVolume < 1.0) //make sure sound cannot go above max
		{
			GameManager.instance.musicVolume = GameManager.instance.musicVolume + 0.1f;
		}

	}
	public void musicDown()//decreases music volume
	{
		if (GameManager.instance.musicVolume > 0.0) //makes sure sound cannot go below 0
		{
			GameManager.instance.musicVolume = GameManager.instance.musicVolume - 0.1f;
		}
	}
	public void sfxUp()//increases sfx volume
	{
		if (GameManager.instance.sfxVolume < 1.0)//makes sure sound effects cannot go above max
		{
			GameManager.instance.sfxVolume = GameManager.instance.sfxVolume + 0.1f;
		}
	}
	public void sfxDown()//decreases sfx volume
	{
		if (GameManager.instance.sfxVolume > 0.0) //make sure sound effects cannot go below 0
		{
			GameManager.instance.sfxVolume = GameManager.instance.sfxVolume - 0.1f;
		}
	}
	public void setSinglePlayer()//sets selectMode to single player
	{
		GameManager.instance.selectMode = GameManager.playMode.singlePlayer;
	}
	public void setMultiplayer()//sets selectMode to multiplayer
	{
		GameManager.instance.selectMode = GameManager.playMode.multiplayer;
	}
	public void useRanMap()//sets use random map to true
	{
		GameManager.instance.useRandomMap =!GameManager.instance.useRandomMap;
	}
	public void useDayMap()//sets use map of the day to true
	{
		GameManager.instance.useMapOfDay = !GameManager.instance.useMapOfDay;
	}
	public void submitName()//called to put user's name and score in high score list
	{
		playerName = scoreListInput.transform.Find ("Text").GetComponent<Text>();
		bool doneMod = false;//used to tell it when to disable input box
		int stringScore=0;
		if (GameManager.instance.selectMode == GameManager.playMode.multiplayer)
		{
			string[] playerNumber =  Regex.Split(whoIsHigh.text.ToString(), @"\s");
			Debug.Log (playerNumber[0]);
			Debug.Log (playerNumber[1]);
			Debug.Log (playerNumber[2]);
			stringScore = int.Parse(playerNumber[1]);//parse the score to an int
			stringScore=stringScore-1;
		}
		scoreFormat=string.Format ("{0,10} {1,6}", "~", GameManager.instance.players[stringScore].GetComponent<TankData>().playerScore);
		//puts users name and score in high score list with a 10 field between name and ~ and a left aligned 6 field for score
		if (modifier.score.Length==1)
		{
			if(scoreList.enabled==false)//make sure text is accessible
			{
				scoreList.enabled=true;
			}

			scoreList.text ="1" + ". " + playerName.text.ToString() + scoreFormat+"\n";
			modifier.lines[0]="1" + ". " + playerName.text.ToString () + scoreFormat+"\n";
			modifier.score[0]= GameManager.instance.players[stringScore].GetComponent<TankData>().playerScore;
			Array.Resize(ref modifier.score, 2);
			modifier.score[1]=-1;//this slot is a empty slot
			doneMod=true;
		}
		else if(modifier.score.Length>1)
		{   
			for (int i=0;i<modifier.score.Length-1;i++)
			{

				if(GameManager.instance.players[stringScore].GetComponent<TankData>().playerScore>modifier.score[i])
				{  

					string newString=(i+1) + ". " + playerName.text.ToString() + scoreFormat+"\n";
					int newInt=GameManager.instance.players[stringScore].GetComponent<TankData>().playerScore;
					for(int r=i;r<modifier.score.Length;r++)//move current score down if needed before replacing
					{

						string tempString;
						int tempInt;
						tempString=modifier.lines[r];//sets temp string to current line
						tempInt=modifier.score[r];//sets temp score to current score in list spot
						modifier.score[r]=newInt;//sets current spot to newInt
						modifier.lines[r]=newString;//sets current string to new string
					    string changeLine=modifier.lines[r];//gets the current line
						string[] pieces=changeLine.Split('.');
						string rep=(r+1)+"."+pieces[pieces.Length-1];
						modifier.lines[r]=rep;
						newInt=tempInt;
						newString=tempString;


					}

					i=modifier.score.Length-2;//used to make sure loop stops after finding a bigger score



					if(scoreList.enabled==false)//make sure text is accessible
					{
						scoreList.enabled=true;
					}

					scoreList.text=" ";
				    for(int n=0;n<modifier.lines.Length;n++)
					 {
						scoreList.text =scoreList.text.ToString()+modifier.lines[n];

						
					 }
				

					
				} 
				else if(i==modifier.score.Length-2&&modifier.score[i+1]==-1)//if last spot is empty and player score is not greater than any currently on list
			  {
					

					modifier.score[i+1]=GameManager.instance.players[stringScore].GetComponent<TankData>().playerScore;
					modifier.lines[i+1]=(i+2) + ". " + playerName.text.ToString() + scoreFormat+"\n";
					if(scoreList.enabled==false)//make sure text is accessible
					{
						scoreList.enabled=true;
					}
					
					scoreList.text=" ";
					for(int n=0;n<modifier.lines.Length;n++)
					{
						scoreList.text =scoreList.text.ToString()+modifier.lines[n];
						
					}
				}

			  }
			    if(modifier.score.Length<10)//if score list is less than 10 keep resizing
			    {
			      Array.Resize(ref modifier.score,modifier.score.Length+1);
				modifier.score[modifier.score.Length-1]=-1;//sets spot to negative so it is known that no player score is in it
			    }
			doneMod=true;
		}
	
		if (doneMod == true) {
			GameManager.instance.disableInputField();
			save.GetComponent<SaveManager> ().Save ();//saves game 
			if (GameManager.instance.selectMode == GameManager.playMode.multiplayer&& scoreListText.GetComponentInParent<scoreListMod> ().runThru==1)
			{
				modifier.enabled=true;
				//makes sure it checks scores a second time for multiplayer
				modifier.scoreState=scoreListMod.scoreDecision.checkScores;
			}

		}
	
	
	    

	}
	public void pressedButton()
	{
		buttonPressed.volume = GameManager.instance.sfxVolume;
		buttonPressed.Play();
	    
	}
	public void releaseButton()
	{
		buttonPressed.Stop ();
		buttonReleased.volume = GameManager.instance.sfxVolume;
		buttonReleased.Play();
	}
}
