using UnityEngine;
using System.Collections;

public class setMenuMusic : MonoBehaviour {
	private AudioSource current;
	// Use this for initialization
	void Start () {
		current = gameObject.GetComponent<AudioSource> ();
		current.clip = audioList.instance.menuMusic;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
