using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerGroundDetector : MonoBehaviour {

	PlatformerController platformerController;
	Animator platformerAnim;

	// Use this for initialization
	void Start () {
		platformerController = GetComponentInParent(typeof (PlatformerController)) as PlatformerController;
		platformerAnim = GetComponentInParent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//if the ground detecting circle collider hits ground
	void OnTriggerEnter2D(Collider2D collider)
	{
		if(!collider.CompareTag("Player") && (collider.name != "CastleLeft") && (collider.name != "CastleRight"))
		{
			platformerController.SetIsGrounded(true);
			platformerAnim.SetBool("Falling", false);
		}
        if (collider.name == "block_jumppad(Clone)")
        {
            //platformerController.ApplyJumpPadPhysics();
        }
        if(collider.gameObject.name != "block_ice(Clone)")
        {
            platformerController.ice = false;
        }
        if (collider.gameObject.name != "block_tar(Clone)")
        {
            platformerController.tar = false;
        }
    }

	//prevents weird behaviours from running across different blocks
	void OnTriggerStay2D(Collider2D collider)
	{
		if(!collider.CompareTag("Player") && (collider.name != "CastleLeft") && (collider.name != "CastleRight"))
		{
			platformerController.SetIsGrounded(true);
			platformerAnim.SetBool("Falling", false);
		}
	}

	//if the ground detecting circle collider leaves ground
	void OnTriggerExit2D(Collider2D collider)
	{
		if(!collider.CompareTag("Player"))
		{
			platformerController.SetIsGrounded(false);
		}
	}
}
