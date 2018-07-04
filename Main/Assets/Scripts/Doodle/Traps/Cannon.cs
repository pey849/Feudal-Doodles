using Doodle.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {
    BoxCollider2D collider2D;
    public GameObject cannonBall;

    public float cooldownTime;
    public bool onCooldown;

    public TeamRole owner;

    GameEngine gameEngine;

    public void setOwner(TeamRole blockOwner)
    {
        owner = blockOwner;
    }

    void Awake () {
        collider2D = gameObject.GetComponent<BoxCollider2D>();
        gameEngine = GameObject.Find("Game Manager").GetComponent<GameEngine>(); //Uncomment this
        owner = gameObject.GetComponent<Block>().role;
        onCooldown = false;
	}
    private void Start()
    {
        owner = gameObject.GetComponent<Block>().role;
        if (owner == TeamRole.PurpleBuilder) //0 for left team
        {
            transform.localScale = new Vector3(transform.localScale.x * 1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    // Update is called once per frame
    void Update () {
        if (! onCooldown)
        {
			//if we are using touch
            if (Input.touchCount > 0  && gameEngine.getTeamRole() == owner)
            {
				//pass in touch position
				fireCannon (Input.GetTouch(0).position);
            }
			//else we are using mouse
			else if (Input.GetMouseButtonDown(0) && gameEngine.getTeamRole() == owner)
			{
				//pass in mouse position
				fireCannon (Input.mousePosition);
			}
        }
        else
        {
            cooldownTime -= Time.deltaTime;
        }

        if(cooldownTime <= 0)
        {
            onCooldown = false;
            cooldownTime = 7;
        }
       
    }

	//takes in a position and checks if we are touching a cannon
	//if so fire the cannon and start the cooldown
	private void fireCannon(Vector2 position){
		
		Vector3 startPos = Camera.main.ScreenToWorldPoint(position); //changes raw coordinates to screen coordinates
		Vector2 touchPos = new Vector2(startPos.x, startPos.y);

		if (collider2D == Physics2D.OverlapPoint(touchPos))
		{
			EventManager.Instance.PostNotification(EventType.Explosion, this, null);
			if (owner==TeamRole.PurpleBuilder) //0 for left team
			{
				Vector3 bulletSpawnPosition = new Vector3(this.transform.position.x + 0.7f, this.transform.position.y, this.transform.position.z);
				cannonBall.transform.position = bulletSpawnPosition;
				if (!gameEngine.getIsHost())
				{
					gameEngine.SendProjectilePacket(cannonBall, owner);
				} else
				{
					gameEngine.SendProjectileFromHost(cannonBall);
				}
			}
			else if (owner == TeamRole.YellowBuilder) //1 for right team
			{
				Vector3 bulletSpawnPosition = new Vector3(this.transform.position.x - 0.7f, this.transform.position.y, this.transform.position.z);
				cannonBall.transform.position = bulletSpawnPosition;
				if (!gameEngine.getIsHost())    
				{
					gameEngine.SendProjectilePacket(cannonBall, owner);
				}
				else
				{
					gameEngine.SendProjectileFromHost(cannonBall);
				}
			}
			onCooldown = true;
		}

	}
}
