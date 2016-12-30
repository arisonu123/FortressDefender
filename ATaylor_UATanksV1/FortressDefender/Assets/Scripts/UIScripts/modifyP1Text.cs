using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class modifyP1Text : MonoBehaviour {
	Text p1Text;
	private TankData data;
	private GameObject playerOne;
	// Use this for initialization
	void Start () {
		playerOne = GameManager.instance.players[0];
		if (data == null)
		{
			data=playerOne.GetComponent<TankData>();
		}
		p1Text = gameObject.GetComponent<Text> ();
		p1Text.text = "Lives: " + GameManager.instance.lives+
			"\nScore: 0"+"\nHealth: "+GameManager.instance.hp;

	}
	
	// Update is called once per frame
	void Update () {
		if(p1Text.enabled==true)
		{
		playerOne = GameManager.instance.players[0];
		p1Text.text = "Lives: " + playerOne.GetComponent<TankData> ().lives +
				"\nScore: " + playerOne.GetComponent<TankData> ().playerScore+"\nHealth: "+playerOne.GetComponent<TankData> ().currentHealth;
		}
	}
}
