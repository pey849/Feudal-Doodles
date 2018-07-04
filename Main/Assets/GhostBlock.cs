using Doodle.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBlock : MonoBehaviour {
    public Block block;
    public Team owner;
    private GameObject gameManager;
    public Sprite myPic;
    public Sprite OpponenetPic;
	public Animator ghostEffect;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.name == "hero 1(Clone)") //&& coll.gameObject.GetComponent<GameEngine>().getTeamRole() == block.role) 
        {
            if(coll.gameObject.GetComponent<PlatformerController>().myTeam == owner)
            {
                gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
            }
			else
			{
				
				ghostEffect.SetTrigger("Fade");
			}         
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.name == "hero 1(Clone)") // && GE.getTeamRole == block.Role
        {
            //gameObject.GetComponent<BoxCollider2D>().enabled = true;
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    // Use this for initialization
    void Start () {
        block = gameObject.GetComponent<Block>();
        gameManager = GameObject.Find("Game Manager");
        owner = Team.Yellow;
        if (block.role == TeamRole.PurpleBuilder || block.role == TeamRole.PurpleRunner)
        {
            owner = Team.Purple;
        }

        if (Common.GetTeam(gameManager.GetComponent<GameEngine>().getTeamRole()) == owner)
        {
            //gameObject.GetComponent<SpriteRenderer>().sprite = myPic;
            gameObject.GetComponent<SpriteRenderer>().sprite = myPic;
        }
        else
        {
            //gameObject.GetComponent<SpriteRenderer>().sprite = OpponenetPic;
            gameObject.GetComponent<SpriteRenderer>().sprite = OpponenetPic;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
