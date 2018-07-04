using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.Game;

public class FlagTrigger : MonoBehaviour 
{
	public Team castleTeam;

    public GameEngine engine;

	public bool IsDebug = false;

	void OnTriggerEnter2D(Collider2D collider)
	{
		// Make sure it was a player that hit the trigger
		if(collider.tag == "Player")
		{
			Team localTeam = this.engine.GetTeam();

			if (IsDebug) Debug.Log("FlagTrigger - Castle Team: " + castleTeam);
			if (IsDebug) Debug.Log("FlagTrigger - Local team: " + localTeam);

			PlatformerController player = collider.GetComponent<PlatformerController>();

			// Only send flag event if I'm the runner dealing with the flag
			if ( ! player.IamThisRunner() )
				return;

			// Throw flag capture (i.e. drop off) event.
			if (player.HasFlag() && castleTeam == localTeam)
			{
                EventManagerNetworked.Instance.BroadcastEvent(EventType.FlagCaptured, new EventManagerArg(localTeam));
			}
			// Throw flag grab event.
			else if( ! player.HasFlag() && castleTeam != localTeam)
			{
                EventManagerNetworked.Instance.BroadcastEvent(EventType.FlagGrabbed, new EventManagerArg(localTeam));
			}
		}
	}
}
