using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SaveManager : MonoBehaviour {
	private GameObject hScore;
	private Text highScore;
	private string[] scores;
	private float musicVolume;
	private float sfxVolume;
	// Use this for initialization
	void Start () {
		musicVolume = GameManager.instance.musicVolume;
		sfxVolume = GameManager.instance.sfxVolume;
		hScore = GameObject.Find ("scoreText");
		highScore = hScore.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		hScore = GameObject.Find ("scoreText");
		highScore = hScore.GetComponent<Text> ();
	}
	public void Save()//used to set keys with information and save game settings and scores
	{
		PlayerPrefs.SetString ("ScoreData", highScore.text);
		PlayerPrefs.SetFloat ("musicVolume", musicVolume);
		PlayerPrefs.SetFloat ("sfxVolume", sfxVolume);
		if (GameManager.instance.useMapOfDay == true) {
			PlayerPrefs.SetInt ("mapOfDay", 1);
		} else {
			PlayerPrefs.SetInt ("mapOfDay", 0);
		}
		if (GameManager.instance.useRandomMap == true) {
			PlayerPrefs.SetInt ("randomMap", 1);
		} else {
			PlayerPrefs.SetInt ("randomMap", 0);
		}
		if(GameManager.instance.selectMode==GameManager.playMode.singlePlayer)
		{
			PlayerPrefs.SetInt("mode",0);
		}
		else
		{
			PlayerPrefs.SetInt ("mode",1);
		}
		PlayerPrefs.Save ();

	}
	public void Load()
	{
		if (PlayerPrefs.HasKey("ScoreData")==true&&PlayerPrefs.HasKey("musivVolume")==true&&PlayerPrefs.HasKey("sfxVolume")==true){
        //if keys exist load info
		highScore.text = PlayerPrefs.GetString ("ScoreData");
		GameManager.instance.musicVolume = PlayerPrefs.GetFloat ("musicVolume");
		GameManager.instance.sfxVolume = PlayerPrefs.GetFloat ("sfxVolume");
		}
		if (PlayerPrefs.HasKey ("mapOfDay") == true) 
		{
			//if key exists loads useMapOfDay option
			int dailyMapOp = PlayerPrefs.GetInt ("mapOfDay");
			if (dailyMapOp == 1){
				GameManager.instance.useMapOfDay = true;
			}
			else
			{
				GameManager.instance.useMapOfDay=false;
		    }
		}
		if (PlayerPrefs.HasKey ("randomMap") == true) {
			int dailyMapOp = PlayerPrefs.GetInt ("randomMap");
			if (dailyMapOp == 1){
				GameManager.instance.useMapOfDay = true;
			}
			else
			{
				GameManager.instance.useMapOfDay=false;
			}
		}
		if (PlayerPrefs.HasKey ("mode") == true) {
			int chosenMode = PlayerPrefs.GetInt ("mode");
			if (chosenMode == 0) {
				GameManager.instance.selectMode = GameManager.playMode.singlePlayer;
			} else {
				GameManager.instance.selectMode = GameManager.playMode.singlePlayer;
			}
		}

	}
}
