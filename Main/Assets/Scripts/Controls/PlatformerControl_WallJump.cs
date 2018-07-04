using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.InputSystem.Handlers;
using Doodle.InputSystem;

/*

	DO NOT USE IN PRODUCTION. THIS IS JUST A TEST WALL JUMP SCRIPT THAT DOESN'T WORK
	SMOOTHLY

*/

public class PlatformerControl_WallJump : MonoBehaviour {

	public float distance = 1f;
	PlatformerControl_Movement movement;
	public float speed=40f;
	bool walljumping;

	//the layermask for wall jumping
	public LayerMask wallLayerMask;

	RaycastHit2D wallHit;

	// Use this for initialization
	void Start () 
	{
		movement = GetComponent<PlatformerControl_Movement> ();
	}


	void FixedUpdate(){

		detectWallHit ();
		//Debug.Log ("Button: " + movement.gamepad.GetButton("Jump"));
		if (!movement.isGrounded && wallHit != null) {
			
			if(movement.gamepad.GetButton(InputButton.Jump) == ButtonState.JustPressed){
				//movement.outsideForce = true;
				GetComponent<Rigidbody2D> ().AddForce(new Vector2 (speed * 4, speed), ForceMode2D.Impulse);
				Debug.Log ("Detected Wall");
				//StartCoroutine ("TurnIt");
			}


		}


	}

	void detectWallHit(){
		Vector2 direction;
		if (movement.isFacingRight) {
			direction = Vector2.right;
		} else {
			direction = Vector2.left;
		}
		wallHit = Physics2D.Raycast(transform.position, direction, 0.3f, wallLayerMask);

	}


}


