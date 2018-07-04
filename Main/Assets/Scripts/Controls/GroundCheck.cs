using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

	You can use this script if you want.  You will need to tweak the 
	PlatformerController.cs script to make the isGrounded variable to be public
	and swap out anywhere that says PlatformerControl_Movement with PlatformerController

*/

//A script to handle checking whether or not the player is on the ground
//put this script on a child game object of the Player.  The Child game object
//should have a box/circle collider near its feet (bottom of the collider line up 
// with the bottom of its feet)
public class GroundCheck : MonoBehaviour {

	//change this line to "private PlatformerController player;"
	private PlatformerControl_Movement player;

	// Use this for initialization
	void Start () {
		//change this line to "player = gameObject.GetComponentInParent<PlatformerController> ();"
		player = gameObject.GetComponentInParent<PlatformerControl_Movement> ();
	}
	
	void OnTriggerEnter2D(Collider2D collider){
		//Debug.Log ("Entered Ground");
		player.isGrounded = true;

	}

	void OnTriggerStay2D(Collider2D collider){
		//Debug.Log ("Staying on Ground");
		player.isGrounded = true;
	}

	void OnTriggerExit2D(Collider2D collider){
		//Debug.Log ("Left Ground");
		player.isGrounded = false;
	}

}
