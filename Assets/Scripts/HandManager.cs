using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
	public GameObject leftHand;
	public GameObject rightHand;

    public GameObject rh_deadzone;
    public GameObject lh_deadzone;

	public float deadZoneSize = 0.3f;
	public float sensitivity = 1.0f;

	public Vector3 leftHandInitialPos;
	public Vector3 rightHandInitialPos;

	public Vector3 rightHandCurrentPos;
	public Vector3 lefthandCurrentPos;

	//negative is down / back / left
	//positive is up / forward / right
	public Quaternion DroneMovement;

	// Start is called before the first frame update
	void Start() {
		//do the intial setup here because i am lazy
		leftHandInitialPos = leftHand.transform.position;
		rightHandInitialPos = rightHand.transform.position;

        rh_deadzone.transform.position = rightHandInitialPos;
        lh_deadzone.transform.position = leftHandInitialPos;

        lh_deadzone.transform.localScale = new Vector3(deadZoneSize, deadZoneSize, deadZoneSize);
        rh_deadzone.transform.localScale = new Vector3(deadZoneSize, deadZoneSize, deadZoneSize);
    }

    // Update is called once per frame
    void Update() {
		Zero(DroneMovement);

		//get current hand positions
		rightHandCurrentPos = rightHand.transform.position;
		lefthandCurrentPos = leftHand.transform.position;

        //get right hand and left hand displacement vector
		Vector3 rightHandDisplacement = (rightHandInitialPos - rightHandCurrentPos) * -1;
		Vector3 leftHandDisplacement = (leftHandInitialPos - lefthandCurrentPos) * -1;

		//dead zone checks
		if(Vector3.Distance(lefthandCurrentPos, leftHandInitialPos) >= deadZoneSize) {
			Debug.DrawRay(leftHandInitialPos, leftHandDisplacement * 1, Color.red);
		} else {
			leftHandDisplacement = Vector3.zero;
		}

		if (Vector3.Distance(rightHandCurrentPos, rightHandInitialPos) >= deadZoneSize) {
			Debug.DrawRay(rightHandInitialPos, rightHandDisplacement * 1, Color.green);
		} else {
			rightHandDisplacement = Vector3.zero;
		}


		/*
		 *	UP-DOWN / PAN LOGIC
		 */

		//get y components
		float lhDispY = leftHandDisplacement.y;
		float rhDispY = rightHandDisplacement.y;

		int sign = Sign(lhDispY * rhDispY);

		switch(sign) {
			case (-1):	//pan
				DroneMovement[1] = (lhDispY - rhDispY) / sensitivity * ((lhDispY > 0) ? 1 : -1);
				break;
			case (1):   //up-down
				DroneMovement[0] = Average(lhDispY, rhDispY);
				break;
			default:	//tee-hee (do nothing)
				break;
		}

		/*
		 * FOR-BACK / ROTATION LOGIC
		 */

		//get x components (important to have the player face in the x-direction at all times to make this simpler
		float lhDispZ = leftHandDisplacement.z;
		float rhDispZ = rightHandDisplacement.z;

		sign = Sign(lhDispZ * rhDispZ);

		switch (sign) {
			case (-1):  //rotate
				DroneMovement[3] = (lhDispZ - rhDispZ) / sensitivity * ((lhDispZ > 0) ? 1 : -1);
				break;
			case (1):   //up-down
				DroneMovement[2] = Average(lhDispZ, rhDispZ);
				break;
			default:    //tee-hee (do nothing)
				break;
		}

		// SLAP our drone movement quaterion onto our managers transform
		// hack the planet
		transform.SetPositionAndRotation(transform.position, DroneMovement);
	}

	int Sign(float i) {
		if(i > 0.0) { return 1; }
		if(i < 0.0) { return -1; }
		return 0;
	}

	/*
	 * is this bad practice, probably... 
	 */
	void Zero(Quaternion q) {
		for(int i = 0; i < 4; i++) {
			q[i] = 0.0f;
		}
	}

	float Average(float f1, float f2) {
		return (f1 + f2) / 2;
	}

	void OnDrawGizmos() {
		// Display the explosion radius when selected
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(leftHandInitialPos, deadZoneSize);
		Gizmos.DrawWireSphere(rightHandInitialPos, deadZoneSize);
	}
}
