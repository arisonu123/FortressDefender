using UnityEngine;
using System.Collections;

public class setVolume : MonoBehaviour {
	private float musicVolume;
	private AudioSource sound;
	// Use this for initialization
	void Start () {
		musicVolume = GameManager.instance.musicVolume;
		sound=gameObject.GetComponent<AudioSource> ();
		sound.volume = musicVolume;
	}
	
	// Update is called once per frame
	void Update () {
	  if (musicVolume != GameManager.instance.musicVolume) {//make sure that volume setting update when changed
			musicVolume=GameManager.instance.musicVolume;
			sound.volume=musicVolume;
		}
	}

}
