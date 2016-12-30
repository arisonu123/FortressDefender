using UnityEngine;
using System.Collections;

public class audioList : MonoBehaviour {
	public static audioList instance;
	[Header("SFXs")]
	public AudioClip impact;
	public AudioClip death;
	public AudioClip shooting;
	public AudioClip buttonSounds;
	public AudioClip pickUpSounds;
	[Header("Music")]
	public AudioClip menuMusic;
	public AudioClip gameMusic;


	void Awake()
	{
		if (instance == null) {//checks if instance of GameManager already exists,and creates one if not
			instance = this;
		} else {
			Debug.LogError ("ERROR: audioList already exists.");//prints error to console if instance exists
			Destroy (gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
