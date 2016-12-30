using UnityEngine;
using System.Collections;

public class shoot : MonoBehaviour
{   
		
	    public GameObject shotShellPrefab;//shell prefab used for making clones
		public float throwForce;//force shell is shot at
	    public float existTime;//used to keep track of how long shell has existed
	    public float shellTimeOut=10;//used to let program know time to destroy shell
		public Vector3 bulletPos;
	    public Vector3 rightDiagonalPos;//used to store right diagonal of forward vector of enemy tank
	    public Vector3 leftDiagonalPos;//used to store left diagonal of forward vector of enemy tank
	    public TankData data;
	    public Rigidbody shellMove;//used to apply force to shell when shot
	    public Transform trans;
	    public Transform visuals;//used to easier find gameObject grandchildren
	    public Transform cannon;//used to get cannon transform for playing sound when shooting
	    private AudioClip firingSound;//used to get firing sound clip
         void Awake()
	   {
		    shellMove = GetComponent<Rigidbody> ();
		  if (data == null)//sets data if it is currently null
		 {
			data = gameObject.GetComponent<TankData>();
		 }


		    
	   }

		// Use this for initialization
		void Start () {
		shellTimeOut = GameManager.instance.timeOutOfShell;
		existTime = 0f;//starting existing time
		throwForce = data.shellForce;
		trans = gameObject.transform;
		if (firingSound == null) 
		{
			firingSound=audioList.instance.shooting;
			
		}
		visuals = gameObject.transform.Find ("Visuals");
		cannon = visuals.transform.Find ("Cannon");
			
		}
		// Update is called once per frame
		void Update ()
		{


		}
	    public void shootShell(Vector3 size)//spawns a shell at bulletpos
	{

		bulletPos = trans.position +trans.forward;//sets bullet pos to forward vector of player/tank
		AudioSource.PlayClipAtPoint (firingSound, cannon.position, GameManager.instance.sfxVolume);
		GameObject currentShell=Instantiate(shotShellPrefab,bulletPos,trans.rotation)as GameObject;//instantiates shell        
		if (gameObject.tag == "playerOne" || gameObject.tag == "playerTwo") {//makes sure tag is only set if a player is firing
			currentShell.tag = gameObject.name;//used to tag the bullet with the name of gameObject shooting it
		}
		currentShell.transform.localScale = size;//sets scale of shell
	}
	public void shootRowOfShells(Vector3 sizes)//spawns three shells, one at bulletpos and one slightly to the left and another slightly to the right
	{
		bulletPos = trans.position +trans.forward;//sets bullet pos to forward vector of player/tank
		rightDiagonalPos = trans.position + (trans.forward + trans.right);
		leftDiagonalPos = trans.position + (trans.forward +(-trans.right));
		AudioSource.PlayClipAtPoint (firingSound, cannon.position, GameManager.instance.sfxVolume);
		GameObject shellOne=Instantiate(shotShellPrefab,bulletPos,trans.rotation)as GameObject;//instantiates shell from forward position of tank
		GameObject shellTwo=Instantiate(shotShellPrefab,rightDiagonalPos,trans.rotation)as GameObject;//instantiates shell from right forward diagonal position of tank
		GameObject shellThree=Instantiate(shotShellPrefab,leftDiagonalPos,trans.rotation)as GameObject;//instantiates shell from left forward diagonal position of tank
		shellOne.transform.localScale = sizes;
		shellTwo.transform.localScale = sizes;//set scale of shells
		shellThree.transform.localScale = sizes;
	}


		
		
}

