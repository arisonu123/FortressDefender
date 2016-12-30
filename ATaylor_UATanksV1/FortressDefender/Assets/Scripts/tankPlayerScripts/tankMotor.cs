using UnityEngine;
using System.Collections;
public class tankMotor : MonoBehaviour 
{
	private CharacterController tankController;//CharacterController Component
	public Transform trans;
	public Vector3 speedVector=Vector3.zero;
	// Use this for initialization
	void Start () {
		tankController = gameObject.GetComponent<CharacterController> (); //stores CharacterController component for quick access
		trans = gameObject.GetComponent<Transform> ();//stores transform component for quick access

	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.GetComponent<CharacterController> ().enabled == true) {
			tankController.SimpleMove (speedVector);
		}
	}
    public void Move(float speed)//used to move tank forward
	{
	     speedVector = trans.forward*speed;//stores forward vector multiplied by speed of tank
		//tankController.SimpleMove (speedVector);//moves tank forward,applies Time.DeltaTime and converts to meters per a second
	}
	public void Stop(){
		speedVector = Vector3.zero;
	}
	public void Rotate(float speed)//used to rotate tank left/right
	{
		Vector3 rotateVector = Vector3.up * speed * Time.deltaTime;//stores up vector multiplied by speed
		trans.Rotate (rotateVector, Space.Self);//moves tank right by one degree per a second in local space
	}

	public bool rotateTowards(Vector3 rotateTo,float rotateSpeed)//rotates tank if possible, returns false if can't due to tank
    //already facing target
	{
		Vector3 vectorToTargetLoc=rotateTo-trans.position;//difference between target position and tank position


		Quaternion targetRotation = Quaternion.LookRotation (vectorToTargetLoc);//quaternion that looks down target vector position

		if (targetRotation != trans.rotation) {
			trans.rotation = Quaternion.RotateTowards (trans.rotation, targetRotation, rotateSpeed * Time.deltaTime);
			//changes rotation so tank is closer to target location, but never turns faster than rotateSpeed
			return true;
		} else //returns false if target rotation has been reached
		{
			return false;
		}

	}
}
