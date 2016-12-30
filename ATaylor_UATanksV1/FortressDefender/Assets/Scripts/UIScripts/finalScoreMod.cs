using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class finalScoreMod : MonoBehaviour {
	Text fScoreText;
	private TankData data;
	private GameObject playerOne;
	private GameObject playerTwo;
	// Use this for initialization
	void Start () {
		playerOne = GameManager.instance.players [0];
		playerTwo = GameManager.instance.players [1];
		if (data == null)
		{
			data=playerOne.GetComponent<TankData>();
		}
		fScoreText = gameObject.GetComponent<Text> ();
		if (GameManager.instance.selectMode == GameManager.playMode.singlePlayer) {
			fScoreText.text = "Your Score: " + playerOne.GetComponent<TankData>().playerScore;
		} else 
		{
			fScoreText.text = "Player 1 Score: " + playerOne.GetComponent<TankData>().playerScore+
				"\nPlayer 2 Score: "+playerTwo.GetComponent<TankData>().playerScore;

		}
	}


	
	// Update is called once per frame
	void Update () {
		playerOne = GameManager.instance.players [0];
		playerTwo = GameManager.instance.players [1];
		if (GameManager.instance.selectMode == GameManager.playMode.singlePlayer) {
			fScoreText.text = "Your Score: " + playerOne.GetComponent<TankData>().playerScore;
		} else 
		{
			fScoreText.text = "Player 1 Score: " + playerOne.GetComponent<TankData>().playerScore+
				"\nPlayer 2 Score: "+playerTwo.GetComponent<TankData>().playerScore;
			
		}
	}
}
