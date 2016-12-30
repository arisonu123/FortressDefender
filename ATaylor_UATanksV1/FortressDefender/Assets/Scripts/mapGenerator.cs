using UnityEngine;
using System.Collections;
using System;
public class mapGenerator : MonoBehaviour {
	public int rows;
	public int cols;
	public GameObject[] gridPrefabs;
	private float roomWidth = 50.0f;
	private float roomHeight = 50.0f;
	public Room[,] grid;//used to keep track of rooms created ,uses two numbers to refer to it in memory
	public bool isMapOfDay;
	public bool isRandomMap;
	public int chosenSeed;


	// Use this for initialization
	void Start () {
		chosenSeed = GameManager.instance.mapSeed;
		rows = GameManager.instance.mapRows;
		cols = GameManager.instance.mapColumns;
		isMapOfDay = GameManager.instance.useMapOfDay;
		isRandomMap = GameManager.instance.useRandomMap;
		gridPrefabs = GameManager.instance.mapTiles;
	
	}
	
	// Update is called once per frame,,,,
	void Update () {

	}

	public GameObject RandomRoomPrefab()//Returns a random room
	{   
		return gridPrefabs [UnityEngine.Random.Range (0, gridPrefabs.Length)];
		

	}
	public void GenerateGrid()//used to generate map grid
	{

		if (isRandomMap == true && isMapOfDay == false) {//sets map to random map based on time
			UnityEngine.Random.seed = DateToInt (DateTime.Now);//sets "random" seed to current time
		} else if (isRandomMap == false && isMapOfDay == true) {//sets map to map of day based on numbers in day
			UnityEngine.Random.seed = DateToInt (DateTime.Now.Date);
		} else {//if both are selected just use random map
			UnityEngine.Random.seed = DateToInt (DateTime.Now);
		}
		if (chosenSeed != 0) {//if a specific seed is entered in game manager use this instead
			UnityEngine.Random.seed = chosenSeed;
		}
		//Clear out the grid
		grid = new Room[cols, rows];
		GameManager.instance.mapGrid = grid;
		//For each grid row...
		for (int i=0; i<rows; i++)
		{
			//for each column in that row
			for (int j=0; j<cols; j++) 
			{
				//Figure out the location
				float xPosition = roomWidth * j;
				float zPosition = roomHeight * i;
				Vector3 newPosition = new Vector3 (xPosition, 0.0f, zPosition);
				//create a new grid at appropiate location
				GameObject tempRoomObj = Instantiate (RandomRoomPrefab (), newPosition, Quaternion.identity)as GameObject;
				//set its parent
				tempRoomObj.transform.parent = this.transform;
				//give the temp room a meaningful name
				tempRoomObj.name = "Room_" + j + "," + i;
				//Get the room object
				Room tempRoom = tempRoomObj.GetComponent<Room> ();
				//open doors as needed
				if (i == 0) {
					//open north doors if on bottom row
					tempRoom.doorNorth.SetActive (false);
				} else if (i == rows - 1) {
					//Otherwise, if doors are on the top row open south doors
					tempRoom.doorSouth.SetActive (false);
				} else {
					//otherwise, this row is in the middle so both north and south open
					tempRoom.doorNorth.SetActive (false);
					tempRoom.doorSouth.SetActive (false);
				}
				if (j == 0) {
					//if first column then east doors are opened
					tempRoom.doorEast.SetActive (false);
				} else if (j == cols - 1) {
					//Otheriwse, if one last column row open west doors
					tempRoom.doorWest.SetActive (false);
				} else {
					//otherwise, we are in middle so both west and east are opened
					tempRoom.doorEast.SetActive (false);
					tempRoom.doorWest.SetActive (false);
				}
				   
				//save it to the grid array
				grid [j, i] = tempRoom;//
				GameManager.instance.mapGrid=grid;
			}
		}

	}
    public int DateToInt(DateTime dateToUse)//adds date and time up and returns it as an int
	{
		int dateToReturn = dateToUse.Year + dateToUse.Month + dateToUse.Day + dateToUse.Hour +dateToUse.Minute + dateToUse.Second + dateToUse.Millisecond;
		return dateToReturn;
	}
	public void clear()//clears grid
	{
		for (int c=0; c<GameManager.instance.mapGrid.GetLength(0); c++) {
			for (int r=0; r<GameManager.instance.mapGrid.GetLength(1); r++) {
				if(GameManager.instance.mapGrid[c,r]!=null)//if not null destroy
				{
				Destroy(GameManager.instance.mapGrid[c,r].gameObject);
				}
				

			}
	
		}
	}

}
