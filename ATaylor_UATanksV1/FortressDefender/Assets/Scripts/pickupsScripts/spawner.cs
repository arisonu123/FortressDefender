using UnityEngine;
using System.Collections;

public class spawner : MonoBehaviour {//attach this script to spawners
	public GameObject[] pickupPrefab;//TODO make this an array, select random pickups instead of same one each time
	public float spawnDelay;
	private GameObject spawnedPickup;
	private float nextSpawnTime;
	private Transform trans;
	private UnityEngine.Random spawnSelect;//used to select a random pickup
	// Use this for initialization
	void Start () {
		spawnDelay = 30;
		trans = gameObject.GetComponent<Transform> ();
		nextSpawnTime = Time.time + spawnDelay;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
		// If it is there is nothing spawns
		if (spawnedPickup == null) 
		{
			// And it is time to spawn
			if (Time.time > nextSpawnTime)
			{
				// Spawn it and set the next time
				int ranNum=UnityEngine.Random.Range(0,pickupPrefab.Length);//gets number to select random pickup
				spawnedPickup = Instantiate (pickupPrefab[ranNum], trans.position, Quaternion.identity) as GameObject;
				nextSpawnTime = Time.time + spawnDelay;
			} 
		} else
		{
			// Otherwise, the object still exists, so postpone the spawn
			nextSpawnTime = Time.time + spawnDelay;
		}
	}
 }


