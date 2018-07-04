using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Doodle.Game;

public class DeathBoundaryScript : MonoBehaviour 
{
	public GameEngine mEngine;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.tag == "Player")
		{	
			// Broacast event to handle player's death.

			PlatformerController diedPlayer = collider.GetComponent<PlatformerController>();

			// Only send death event if I'm the runner that died
			if ( diedPlayer.IamThisRunner() )
				EventManagerNetworked.Instance.BroadcastEvent(EventType.PlayerDied, new EventManagerArg(diedPlayer.myTeam));
		}
	}
}
