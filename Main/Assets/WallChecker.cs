using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.InputSystem;

public class WallChecker : MonoBehaviour {
    PlatformerController parentController;
    public int blocksInCollider;

    void OnTriggerEnter2D(Collider2D coll)
    {
		if(coll.transform.name != "Cliffside")
		{
			if (coll.GetComponent<Block>().type == "ghost" && coll.GetComponent<GhostBlock>().owner != parentController.myTeam)
       	 	{
        	    return;
       		}
        	else
        	{
          	  parentController.wallHit = true;
          	  blocksInCollider++;
        	}
		}
		else
        {
      	  parentController.wallHit = true;
      	  blocksInCollider++;
    	}
    }

    void OnTriggerExit2D(Collider2D coll)
    {
		if(coll.transform.name != "Cliffside")
		{
        	if (coll.GetComponent<Block>().type == "ghost" && coll.GetComponent<GhostBlock>().owner != parentController.myTeam)
        	{
         	   return;
        	}
        	else
        	{
         	   blocksInCollider--;
         	   if (blocksInCollider < 1)
         	   {
         	       parentController.wallHit = false;
         	       blocksInCollider = 0;
         	   }
        	}
		}
		else
    	{
      	  	blocksInCollider--;
     	    if (blocksInCollider < 1)
     	    {
     	       parentController.wallHit = false;
     	       blocksInCollider = 0;
     	    }
    	}
    }

	// Use this for initialization
	void Start () {
        parentController = gameObject.GetComponentInParent<PlatformerController>();
        blocksInCollider = 0;
	}
	
	// Update is called once per frame
	void Update () {
	}
}
