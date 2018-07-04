using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.InputSystem.Handlers;
using Doodle.InputSystem;

/*

	DO NOT USE IN PRODUCTION. THIS IS JUST A TEST MOVEMENT SCRIPT

*/

public class PlatformerControl_Movement : MonoBehaviour {


	public Rigidbody2D rb;

	//SpeedVariables
	public float speed = 5f;

	public bool isGrounded;

	//the platformer player's gamepad input handler
	public GamepadInputHandler gamepad;

	//the sprite renderer component
	private SpriteRenderer sprite;


	float xAxis;
	//is the player facing right?
	public bool isFacingRight;
	public float jumpForce;


	public bool wallSliding = false;
	public float wallSlideSpeedMax = 3.0f;
	public bool wallCheck;
	public LayerMask wallLayerMask;


	[HideInInspector]
	public List<Collider2D> colliderList;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		gamepad = gameObject.GetComponent (typeof(Doodle.InputSystem.InputHandler)) as GamepadInputHandler;
		sprite = gameObject.GetComponent( typeof(SpriteRenderer) ) as SpriteRenderer;
		isFacingRight = true;
		colliderList = new List<Collider2D> ();
	}

	// Update is called once per frame
	void Update () {
		
		//check which direction we are facing
		directionFacing (gamepad);
		float pushOffFacingDirection = isFacingRight ? 1 : -1;


		if(gamepad.GetButton(InputButton.Jump) == ButtonState.JustPressed){

			if (isGrounded) {
				rb.AddForce (new Vector2 (0, jumpForce), ForceMode2D.Impulse);
				//rb.velocity = new Vector2(80,jumpForce);
				wallSliding = false;
				rb.drag = 0;
				Debug.Log ("is jumped");
			}
		}


		//wall sliding check and setting sliding values
		if (!isGrounded && wallCheck && rb.velocity.y < 0) {
			//Debug.Log (rb.velocity.y);
			wallSliding = true;

			if (rb.velocity.y < (wallSlideSpeedMax*-1)) {
				//rb.velocity = new Vector2(rb.velocity.x,(wallSlideSpeedMax*-1));
				rb.drag = 15f;
			}

		}

	}
		

	void FixedUpdate(){
		xAxis = gamepad.GetAxis (InputAxis.MotionX);
		//check if we hitting a wall, this should check left and right the way we are facing
		rb.velocity = new Vector2 (xAxis * speed, rb.velocity.y);

	}
		

	void directionFacing(GamepadInputHandler g){

		float direction = g.GetAxis (InputAxis.MotionX);
		//Debug.Log ("Direction: " + direction);
		if (direction > 0) {
			isFacingRight = true;
			//sprite.flipX = true;
			transform.localScale = new Vector3 (1,1,1);
			//wallCheckPoint.localScale = new Vector2 (wallCheckPoint.localScale.x * 1, wallCheckPoint.localScale.y);
		} else if (direction < 0) {
			isFacingRight = false;
			//sprite.flipX = false;
			transform.localScale = new Vector3 (-1,1,1);
			//wallCheckPoint.localScale = new Vector2 (wallCheckPoint.localScale.x * -1, wallCheckPoint.localScale.y);
		} else {
			//just keep the last known variables
		}

	}
}
