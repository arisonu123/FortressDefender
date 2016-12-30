using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class modifyP2Text : MonoBehaviour {
	Text p2Text;
	private TankData data;
	private GameObject playerTwo;
	// Use this for initialization
	void Start () {
		playerTwo = GameManager.instance.players [1];
		if (data == null) {
			data = playerTwo.GetComponent<TankData> ();
		}
		p2Text = gameObject.GetComponent<Text> ();
		p2Text.text = "Lives: " +GameManager.instance.lives +
			"\nScore: 0"+"\nHealth: "+GameManager.instance.hp;
	}
	
	// Update is called once per frame
	void Update () {
		if (p2Text.enabled == true) {
			playerTwo = GameManager.instance.players [1];
			p2Text.text = "Lives: " + playerTwo.GetComponent<TankData> ().lives +
				"\nScore: " + playerTwo.GetComponent<TankData> ().playerScore+"\nHealth: "+playerTwo.GetComponent<TankData> ().currentHealth;
		}
	}
}
