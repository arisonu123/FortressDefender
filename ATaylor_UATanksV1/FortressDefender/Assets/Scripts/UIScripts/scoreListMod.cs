using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;//needed to use Regex to get numbers from strings
using UnityEngine.UI;

public class scoreListMod : MonoBehaviour {
	private GameObject isHighText;
	private Text whoIsHigh;
	public enum scoreDecision{inputName,checkScores};
	public scoreDecision scoreState;
	public string[] lines;//used to keep track of each line
	public int[]score;//used to keep track of scores
	public int runThru;//used to keep track of how many times scores have been checked to avoid same player putting thier name in two spots in one session
	public int l;//used to keep track of line number
	// Use this for initialization
	void Start () {

		scoreState=scoreDecision.checkScores;
		isHighText = GameObject.Find ("isHighScore");
		whoIsHigh = isHighText.GetComponent<Text> ();
		runThru = 0;
		lines=new string[10];
		score=new int[1];
	}
	
	// Update is called once per frame
	void Update () {

		if (scoreState == scoreDecision.checkScores) { //check scores to see if player made high score list
			//check for transitions
			if(runThru!=1&&GameManager.instance.selectMode==GameManager.playMode.singlePlayer){
			   runThru=1;
			}
			else if(runThru<2&&GameManager.instance.selectMode==GameManager.playMode.multiplayer)
			{
				runThru=runThru+1;
			}
			if (score.Length< 10) { //if numScores>10 switch scoreState to inputName
				if (runThru == 1) {
					if (GameManager.instance.selectMode == GameManager.playMode.singlePlayer) {
						whoIsHigh.text = "You have acheieved a high score!";
						scoreState = scoreDecision.inputName;
					} else if (GameManager.instance.players[0].GetComponent<TankData> ().playerScore >= GameManager.instance.players [1].GetComponent<TankData> ().playerScore && GameManager.instance.selectMode == GameManager.playMode.multiplayer) {
						whoIsHigh.text = "Player 1 Has Acheived A High Score!";
						scoreState = scoreDecision.inputName;
					} else {
						whoIsHigh.text = "Player 2 Has Acheived A High Score!";
						scoreState = scoreDecision.inputName;
					}

				} else if (runThru == 2) {//makes sure player does not put thier name on highscore list more than once in a session
					if (whoIsHigh.text == "Player 1 Has Acheived A High Score!") {
						whoIsHigh.text = "Player 2 Has Acheived A High Score!";
						scoreState = scoreDecision.inputName;
					} else {
						whoIsHigh.text = "Player 1 Has Acheived A High Score!";
						scoreState = scoreDecision.inputName;
					}


				}
			}
			if (score.Length == 10) {//if high score list contains 10 scores already
				for (l=0; l<score.Length; l++)
				{//loops through each score

					if ((score[l] < GameManager.instance.players [0].GetComponent<TankData> ().playerScore) && GameManager.instance.selectMode == GameManager.playMode.singlePlayer) {//if score is higher than one already in list
						whoIsHigh.text = "You Have Acheived A High Score!";
						scoreState = scoreDecision.inputName;
					}
					if (score[l] < GameManager.instance.players [0].GetComponent<TankData> ().playerScore && GameManager.instance.selectMode == GameManager.playMode.multiplayer && score[l]> GameManager.instance.players [1].GetComponent<TankData> ().playerScore) {//if p1 score is higher than one already in list
						whoIsHigh.text = "Player 1 Has Acheived A High Score!";
						scoreState = scoreDecision.inputName;
					}
					if (score[l] < GameManager.instance.players [1].GetComponent<TankData> ().playerScore && GameManager.instance.selectMode == GameManager.playMode.multiplayer && score[l]> GameManager.instance.players [0].GetComponent<TankData> ().playerScore) {//if p2 score is higher than one already in list
						whoIsHigh.text = "Player 2 Has Acheived A High Score!";
						scoreState = scoreDecision.inputName;
					}
					if (score[l] < GameManager.instance.players [0].GetComponent<TankData> ().playerScore && GameManager.instance.selectMode == GameManager.playMode.multiplayer && score[l] < GameManager.instance.players [1].GetComponent<TankData> ().playerScore) {//if p1 and p2 score is higher than one already in list
						if (GameManager.instance.players [0].GetComponent<TankData> ().playerScore > GameManager.instance.players [1].GetComponent<TankData> ().playerScore && runThru == 1) {
							whoIsHigh.text = "Player 1 Has Acheived A High Score!";
							scoreState = scoreDecision.inputName;
						} else if (GameManager.instance.players [1].GetComponent<TankData> ().playerScore > GameManager.instance.players [0].GetComponent<TankData> ().playerScore && runThru == 1) {

							whoIsHigh.text = "Player 2 Has Acheived A High Score!";
							scoreState = scoreDecision.inputName;
		
						} else if (runThru == 2) {//ensures a player does not put thier score on high score list more than once in a game session
							if (whoIsHigh.text == "Player 1 Has Acheived A High Score!") {
								if(score[l] < GameManager.instance.players [1].GetComponent<TankData> ().playerScore){// if current score in list is less than player 2 score
								whoIsHigh.text = "Player 2 Has Acheived A High Score!";
								scoreState = scoreDecision.inputName;
								}
							} else {
								if(score[l] < GameManager.instance.players [0].GetComponent<TankData> ().playerScore)//if current score in list less than player one score
								{
								whoIsHigh.text = "Player 1 Has Acheived A High Score!";
								scoreState = scoreDecision.inputName;
								}
							}
							
							
						}
					}


				}
			
				
		}

		if (scoreState == scoreDecision.inputName) {//enables users ability to input name
			  

				GameManager.instance.enableInputField();

		
		}

	}
 }
	public void setRunThru()
	{
		runThru = 0;
	}
	
}
