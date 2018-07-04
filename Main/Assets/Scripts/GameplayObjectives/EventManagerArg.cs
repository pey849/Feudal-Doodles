using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.Game;

public class EventManagerArg : Object
{
	private string arg;

	/* Create a generic object argument to pass to EventManager or EventManagerNetworked */

	public EventManagerArg(string arg)
	{
		this.arg = arg;
	}

	public EventManagerArg(Team arg)
	{
		this.arg = arg.ToString();
	}

	/* Convert back to a specific type */

	public string ToString()
	{
		return this.arg;
	}

	public Team ToTeam()
	{
		return (Team) System.Enum.Parse(typeof(Team), this.arg);
	}
}
