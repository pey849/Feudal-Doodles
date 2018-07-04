using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.InputSystem.Handlers;
using Doodle.InputSystem;
using Doodle.Networking.Packets;
using System;

//A script to handle blowing up blocks that are within a player's circle collider
//put this script on a child game object of the Player.  The Child game object
//should have a circle collider and make sure the "Is Trigger" is on. 
public class PlatformerController_BlockExplosion : MonoBehaviour {

	//Throw in a player who has the PlatformerController Script attached to it
	public PlatformerController player;
	//A blast list to keep track of all the objects that are within our blast radius (our circle collider)
	private List<Collider2D> colliderList;

    public bool explodeOnCooldown;
    public float explodeCooldownTime;

    public float coolDown = 3.0f;

    public void explodeCooldown()
    {
        if (explodeOnCooldown)
        {
            explodeCooldownTime -= Time.deltaTime;
        }

        if (explodeCooldownTime <= 0)
        {
            explodeOnCooldown = false;
        }
    }


    // Use this for initialization
    void Start () {
        explodeOnCooldown = false;
		//initialize the player
		player = gameObject.GetComponentInParent<PlatformerController> ();
		//initialize the blast list
		colliderList = new List<Collider2D> ();
	}
	
	void Awake() {
		player = gameObject.GetComponentInParent<PlatformerController> ();
        explodeOnCooldown = false;
	}

	void Update(){
        //Tick Cooldown
        explodeCooldown();
        //If we pressed "X" button - **this button will probably get changed later
		if(player.gamepad.GetButton(InputButton.Grab) == ButtonState.JustPressed && explodeOnCooldown == false){
            explodeOnCooldown = true;
            explodeCooldownTime = coolDown;
			//Debug.Log ("Smashing Basil!");
			//if our list is not empty
			if (colliderList.Count > 0) {
                //destory each game object that is within our circle collider
                for (int i = 0; i < colliderList.Count; i++)
                {
                    try
                    {
                        GameState.Block b2 = new GameState.Block();
                        b2.position = new Vector3(colliderList[i].transform.position.x, colliderList[i].transform.position.y, 6f);
                        GameObject.Find("Game Manager").GetComponent<GameEngine>().sendBlockUpdate(b2, false);
                    } catch (Exception e)
                    {
                        //can't solve this error right now, doesn't seem to be game breaking.
                        print("uh ohh...");
                    }
                }
                colliderList.RemoveRange(0, colliderList.Count);
			}

		}

	}

	//When an object enters our circle collider
	void OnTriggerEnter2D(Collider2D collider){
		//if what we are colliding with is not in our list
		if(!colliderList.Contains(collider)){
			//we are only destorying blocks or whatever we decide we are destorying
			if (collider.CompareTag ("Block")) {
				//add that collider to the blast list
				colliderList.Add (collider);
				//Debug.Log ("Count: " + colliderList.Count);
				//Debug.Log ("Item Added: " + collider.name);
			}
		}

	}

	//When an object exits our circle collider
	void OnTriggerExit2D(Collider2D collider){
		//if the collider(s) leaves our blast radius (circle collider)
		if(colliderList.Contains(collider)){
			//remove it from our blast list so it doesn't get destoryed
			colliderList.Remove (collider);
			//Debug.Log ("Count: " + colliderList.Count);
			//Debug.Log ("Item Removed: " + collider.name);
		}
	}
}
