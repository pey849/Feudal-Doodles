using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.InputSystem.Handlers;
using Doodle.InputSystem;
using Doodle.Game;
using UnityEngine.SceneManagement;

public class PlatformerController : MonoBehaviour, IGameListener {

	#region variables

	//the platformer player's rigidbody
	Rigidbody2D rBody; 

	//is the player grounded?
	bool isGrounded;

	//has the player wall jumped?
	bool hasWallJumped;

	//does the player have the other team's flag?
	bool hasFlag;

    //Has flipped size.
    bool sideCheck;
	//the jump force
	public Vector2 jumpForce;

	//the wall jump force
	public Vector2 wallJumpForce;

    //Force to be applied when boosting off jumppad.
    public Vector2 jumpPadForce;

    //Amount to slow player by when on tar.
    public float tarSlowPercent;

    //Amount to speed player by when on ice.
    public float iceSpeedPercent;

	//the x axis of the joystick
	float xAxis;

	//the maximum running velocity
	public float maxRunVelocity;

	//the maximum velocity in the air
	public float maxAirVelocity;

	//is the player facing right?
	private bool isFacingRight;

	//the platformer player's gamepad input handler
	public GamepadInputHandler gamepad;

	//the layermask for wall jumping
	public LayerMask wallLayerMask;

	public Team myTeam;

	//the sprite renderer of the flag
	public SpriteRenderer flagSprite;

	//the coin particle system
	public ParticleSystem coinParticles;

    //the animator attached to this player
    private Animator anim;

	public ParticleSystem particles;

    //Booleans for if the player is on ice or tar. Used to change physics applied.
    public bool ice;

    public bool tar;

    //Variables used for push cooldown
    public bool pushOnCooldown;
    public float pushCooldownTime;

    public float coolDown = 5.0f;

    public bool wallHit;

    //Finding explosion indicator to disable when on cd
    public PlatformerController_BlockExplosion explosionIndicator;

    public SpriteRenderer indicatorSprite;
    #endregion

    public void pushCooldown()
    {
        if (pushOnCooldown)
        {
            pushCooldownTime -= Time.deltaTime;
        }

        if (pushCooldownTime <= 0)
        {
            pushOnCooldown = false;
        }
    }

    // Use this for initialization
    void Start () 
	{
        explosionIndicator = gameObject.GetComponentInChildren<PlatformerController_BlockExplosion>();
        indicatorSprite = gameObject.transform.GetChild(6).GetComponent<SpriteRenderer>();

        //bool changed from WallChecker for wall jumping
        wallHit = false;
        pushOnCooldown = false;
		//set component variables
		rBody = gameObject.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
		//gamepad = gameObject.GetComponent( typeof(Doodle.InputSystem.InputHandler) ) as GamepadInputHandler;
		//particles = gameObject.GetComponentInChildren<ParticleSystem>();
		flagSprite.enabled = false;
		coinParticles.Stop();
		
		//set booleans to default values
		isFacingRight = false;
		hasFlag = false;

		//subscribe to the player died event
		EventManager.Instance.AddGameListener(EventType.PlayerDied, this);

        // add reference to the animator component
        anim = gameObject.GetComponent<Animator>();

        Application.runInBackground = true;

        rBody.drag = 0f;
        rBody.mass = 5;
        rBody.gravityScale = 2;
        maxRunVelocity = 15000;
        maxAirVelocity = 18;
    }

	// Update is called once per frame
	void Update () 
	{
        
        if(isGrounded && rBody.velocity.y < 0)
        {
            isGrounded = false;
			//if(!anim.GetBool("Falling")){
			//	anim.SetBool("Falling", true);
			//}
        }

        if(explosionIndicator.explodeOnCooldown)
        {
            indicatorSprite.enabled = false;
        }
        else
        {
            indicatorSprite.enabled = true;
        }

        //Update cooldown
        pushCooldown();

        sideCheck = isFacingRight;
		//Get x-axis information for player movement
		xAxis = gamepad.GetAxis(InputAxis.MotionX);

		anim.SetFloat("Speed", Mathf.Abs(rBody.velocity.x));

		if(rBody.velocity.y < -2f){
			if(!anim.GetBool("Falling")){
				anim.SetBool("Falling", true);
			}
		}

        //if (xAxis > 0)
        //{
        //    if (rBody.velocity.x < 0 && isGrounded && ice != true)
        //    {
        //        rBody.velocity = new Vector2((((float)rBody.velocity.x) / 1.3f), rBody.velocity.y);
        //    }
        //}

        //if (xAxis < 0)
        //{
        //    if (rBody.velocity.x > 0 && isGrounded && ice != true)
        //    {
        //        rBody.velocity = new Vector2((((float)rBody.velocity.x) / 1.3f), rBody.velocity.y);
        //    }
        //}

        //if the rigidbody velocity is in certain direction, change to that direction
        if (rBody.velocity.x < -0.1f) //|| xAxis < 0)
		{
			//make sure sprite is flipped back
			//sprite.flipX = false;
			isFacingRight = false;
            transform.localScale = new Vector3(-0.9f,0.9f,1);
			if(!particles.isEmitting && isGrounded) {
				particles.Play();
			}

		}
        else if (rBody.velocity.x > 0.1f) //|| xAxis > 0)
		{
			//flip sprite
			//sprite.flipX = true;
			isFacingRight = true;
            transform.localScale = new Vector3(0.9f, 0.9f, 1);
			if(!particles.isEmitting && isGrounded) {
				particles.Play();
			}
        }
		else
		{
			particles.Stop();
		}

        if(isFacingRight != sideCheck)
        {
            //print("swapped sides");
            GetComponentInChildren<WallChecker>().blocksInCollider = 0;
            wallHit = false;
        }

		//if the player is facing right, check for walls on appropriate sides
		if(isFacingRight)
		{
			//raycast to detect walls for wall jumping
			//wallHit = Physics2D.Raycast(transform.position, Vector2.right, 0.3f, wallLayerMask);
			wallJumpForce.x = (-1)*Mathf.Abs(wallJumpForce.x);
		} 
		else 
		{
			//raycast to detect walls for wall jumping
			//wallHit = Physics2D.Raycast(transform.position, Vector2.left, 0.3f, wallLayerMask);
			wallJumpForce.x = Mathf.Abs(wallJumpForce.x);
		}

        if ((!wallHit && gamepad.GetButton(InputButton.Sprint) == ButtonState.JustPressed) || Input.GetKeyDown(KeyCode.E) && pushOnCooldown == false)
        {
            pushOnCooldown = true;
            pushCooldownTime = coolDown;
            var hitPlayer = Physics2D.RaycastAll(transform.position, Vector2.right, 1);
            if (isFacingRight)
            {
                hitPlayer = Physics2D.RaycastAll(transform.position, Vector2.right, 1);
                Debug.DrawRay(transform.position, Vector2.right, Color.green, 20);
            } else
            {
                hitPlayer = Physics2D.RaycastAll(transform.position, Vector2.left, 1);
                Debug.DrawRay(transform.position, Vector2.left, Color.green, 20);
            }

            for (int i = 0; i < hitPlayer.Length; i++)
            {
                RaycastHit2D hit = hitPlayer[i];
                if (hit.collider.name == "hero 1(Clone)" && hit.collider.gameObject != this.gameObject)
                {
                    if (isFacingRight)
                    {
						GameObject.Find("Game Manager").GetComponent<GameEngine>().SendPushPacket(new Vector2(10, 5));
                    } else
                    {
						GameObject.Find("Game Manager").GetComponent<GameEngine>().SendPushPacket(new Vector2(-10, 5));
                    }
                    print("trying to send push packet");
                }
            }
        }

        //set player velocity based on x axis
        //If player is on a certain block, apply appropriate force.
        //if (tar == true)
        //{
        //    rBody.AddForce(new Vector2(xAxis * maxRunVelocity * Time.deltaTime * tarSlowPercent, rBody.velocity.y));
        //}
        //else if (ice == true)
        //{
        //    rBody.AddForce(new Vector2(xAxis * maxRunVelocity * Time.deltaTime * iceSpeedPercent, rBody.velocity.y));
        //}
        //else
        //{
        //    rBody.AddForce(new Vector2(xAxis * maxRunVelocity * Time.deltaTime, rBody.velocity.y));
        //}

        


        if (rBody.velocity.x > 6)
        {
            rBody.velocity = new Vector2(6, rBody.velocity.y);
        }
        if (rBody.velocity.x < -6)
        {
            rBody.velocity = new Vector2(-6, rBody.velocity.y);
        }

        if (rBody.velocity.y > 11)
        {
            rBody.velocity = new Vector2(rBody.velocity.x, 11);
        }
        if (rBody.velocity.y < -11)
        {
            rBody.velocity = new Vector2(rBody.velocity.x, -11);
        }

        //if (xAxis == 0 && ice != true)
        //{
        //    rBody.velocity = new Vector2((((float)rBody.velocity.x) / 1.25f), rBody.velocity.y);
        //    //print(rBody.velocity.x);
        //}

        //if the player is standing on the ground
        if (isGrounded)
		{
			//if not on ground, play jump animation
			//anim.SetBool("Jumping", false);
			

            rBody.AddForce(new Vector2(xAxis * maxRunVelocity * Time.deltaTime, rBody.velocity.y));
            //set linear drag for normal play and reset wall jump

            hasWallJumped = false;

			//set the particles angle to up
			particles.transform.rotation = new Quaternion(0, -90, 0, particles.transform.rotation.w);

            //if the player pressed jump
            if (gamepad.GetButton(InputButton.Jump) == ButtonState.JustPressed)
            {
				//if not on ground, play jump animation
				//anim.SetBool("Jumping", true);
				anim.SetTrigger("Jumping");

                //add force to rigidbody to 'Jump'
                rBody.AddForce(jumpForce, ForceMode2D.Impulse);
                EventManager.Instance.PostNotification(EventType.PlayerJump, gameObject.transform, null);
            }
        }
		else
		{
            //EventManager.Instance.PostNotification(EventType.PlayerJump, this, null);

            //if the player is up against a wall
            if (wallHit)
			{
				//reset has wall jumped
				hasWallJumped = false;

                //set the particles angle to up
                particles.transform.rotation = new Quaternion(-80, -90, 0, particles.transform.rotation.w);

				if(!particles.isEmitting && wallHit) {
					particles.Play();
				}

				//set linear drag higher for wall slide effect
				//rBody.drag = 15f;
				
				//if the player presses the jump button, wall jump
				if(gamepad.GetButton(InputButton.Jump) == ButtonState.JustPressed)
				{
                    rBody.velocity = new Vector2(0, 0);
					//reset drag and hasWallJumped
					rBody.drag = 0;
					hasWallJumped = true;

					particles.Stop();
					
					//if not on ground, play jump animation
					anim.SetTrigger("Jumping");
		
					//add force to rigidbody to 'Wall Jump'
					rBody.AddForce(wallJumpForce, ForceMode2D.Impulse);
				}
				else
				{
                    //if (isFacingRight && xAxis > 0) {
                    //    rBody.velocity = new Vector2(0,rBody.velocity.y);
                    //}
                    //else if(!isFacingRight && xAxis < 0){
                    //    Debug.Log("What the fukc is happening");
                    //    rBody.velocity = new Vector2(0, rBody.velocity.y);
                    //}
                    //else {
                    //    //allows the player to move off the wall
                    //    rBody.AddForce(xAxis * (new Vector2(30, 0)), ForceMode2D.Force);
                    //}
                    rBody.AddForce(xAxis * (new Vector2(30, 0)), ForceMode2D.Force);
					
                }
            } 
			//else if the player has activated wall jump
			else if(hasWallJumped)
			{           
                //reset linear drag
                rBody.drag = 0f;

				//stop particles from playing
				particles.Stop();
			
				//if the desired movement is less than the max air velocity
				if(xAxis * rBody.velocity.x < maxAirVelocity)
				{
                    //add forces for smoother movement
                    rBody.AddForce(Vector2.right * xAxis * 140);
				}
				//if the absolute value of the velocity is greater than the max air velocity
				if(Mathf.Abs(rBody.velocity.x) > maxAirVelocity)
				{
					//lock out at the max air velocity
					rBody.velocity = new Vector2(Mathf.Sign(rBody.velocity.x) * maxAirVelocity, rBody.velocity.y);
				}
			} 
			else 
			{
				//stop particles from playing
				particles.Stop();

				rBody.drag = 0f;
                //rBody.velocity = new Vector2(xAxis * maxRunVelocity * Time.deltaTime, rBody.velocity.y);
                //if the desired movement is less than the max air velocity
                if (xAxis * rBody.velocity.x < maxAirVelocity)
                {
                    //add forces for smoother movement
                    rBody.AddForce(Vector2.right * xAxis * 140);
                }
                //if the absolute value of the velocity is greater than the max air velocity
                if (Mathf.Abs(rBody.velocity.x) > maxAirVelocity)
                {
                    //lock out at the max air velocity

                    rBody.velocity = new Vector2(Mathf.Sign(rBody.velocity.x) * maxAirVelocity, rBody.velocity.y);
                }
            }
		}
	}

	public void SetAnimation() {
		anim.SetBool("Falling", false);
	}

	#region setters/getters/events/housekeeping

	//sets whether or not the player is grounded
	public void SetIsGrounded(bool isGrounded)
	{
		this.isGrounded = isGrounded;
	}

	//sets whether or not the player has the flag
	//and enables the sprite attached for the flag
	public void SetHasFlag(bool gotFlag)
	{
		//if we are getting the flag, enable the flag sprite
		if(gotFlag == true)
		{
			flagSprite.enabled = true;
			coinParticles.Play();
			//EventManager.Instance.PostNotification(EventType.FlagCaptured, gameObject.transform, null);
		}
		//else disable the flag sprite
		else
		{
			flagSprite.enabled = false;
			coinParticles.Stop();
		}
		hasFlag = gotFlag;
	}

	//returns whether or not the player has the opponents flag
	public bool HasFlag()
	{
		return this.hasFlag;
	}

	//handles the events subscribed to
	public void OnEvent(EventType eventType, Component sender, object arg = null)
	{
		switch(eventType)
		{
			// Respawn myself if something told me that I should die (e.g. DeathBoundaryScript.cs)
			case EventType.PlayerDied:
                Team playerDiedTeam = ((EventManagerArg)arg).ToTeam();
                if (playerDiedTeam == myTeam)
                    RespawnPlayer();
			break;
		}
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name == "spikes(Clone)")
        {
			Debug.Log("Collided with spike");

			// Only send death event if I'm the runner that died
			if ( IamThisRunner() )
            	EventManagerNetworked.Instance.BroadcastEvent(EventType.PlayerDied, new EventManagerArg(myTeam));
        }
        if(coll.gameObject.name == "block_jumppad(Clone)")
        {
            EventManager.Instance.PostNotification(EventType.JumpPad, this, null);
        }
    }

    //handles respawning of the player upon death
    void RespawnPlayer()
	{
        //reset position and remove flag
        //transform.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, transform.position.z);
        
        particles.Stop();
		if(myTeam == Team.Purple)		// Left team.
		{
			transform.position = new Vector3(-3, 4, 6);
			rBody.velocity = Vector2.zero;
		}
		else
		{
			transform.position = new Vector3(32, 4, 6);
			rBody.velocity = Vector2.zero;
		}
		SetHasFlag(false);
	}

    //Function to get called when player steps on a jump pad
    public void ApplyJumpPadPhysics()
    {
        //stop player's velocity to prevent force build ups
        rBody.velocity = new Vector2(rBody.velocity.x, 0);
        
        //Apply jump pad force.
        rBody.AddForce(jumpPadForce, ForceMode2D.Impulse);
    }

	public bool IamThisRunner()
	{
		Team localTeam = GameObject.Find("Game Manager").GetComponent<GameEngine>().GetTeam();
		Role localRole = GameObject.Find("Game Manager").GetComponent<GameEngine>().GetRole();

		return (localRole == Role.Runner && this.myTeam == localTeam);
	}

	#endregion
}
