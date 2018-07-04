using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;  
using System.IO;  
using System.Net;  
using System.Text;  
using Doodle.Game;

/// <summary>
/// Put all your common static functions here like generating a random number. 
/// </summary>
public class Common
{
	public static string GetPublicIpAddress()
	{
		return new WebClient().DownloadString("http://icanhazip.com");            
	}

	public static TeamRole AppendBuilderRole(Team team)
	{
		if (team == Team.Purple)
			return TeamRole.PurpleBuilder;
		else 
			return TeamRole.YellowBuilder;
	}

	public static Team GetTeam(TeamRole teamRole)
	{
		if (teamRole == TeamRole.PurpleRunner || teamRole == TeamRole.PurpleBuilder)
			return Team.Purple;
		else 
			return Team.Yellow;
	}

	public static Role GetRole(TeamRole teamRole)
	{
		if (teamRole == TeamRole.PurpleRunner || teamRole == TeamRole.YellowRunner)
			return Role.Runner;
		else 
			return Role.Builder;
	}
}
