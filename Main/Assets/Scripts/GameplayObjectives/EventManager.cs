using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

	#region properties

	//the public access for the instance of the event manager
	public static EventManager Instance
	{
		get{return instance;}
		set{}
	}

	#endregion

	#region variables
	//singleton event manager instance
	private static EventManager instance = null;

	//the array of game listeners or registered objects for events
	private Dictionary<EventType, List<IGameListener>> GameListeners = new Dictionary<EventType, List<IGameListener>>();

	#endregion

	#region methods
	void Awake()
	{
		//if no instance of this manager exists, then assign this instance
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		} 
		else 
		{
			DestroyImmediate(this);
		}
	}

	//the function to add a game listener to the array of game listeners
	public void AddGameListener(EventType eventType, IGameListener gameListener)
	{
		//the list of game listeners for this event
		List<IGameListener> listenList = null;

		//check for an existing event type key, and if it exists add to that list
		if(GameListeners.TryGetValue(eventType, out listenList))
		{
			listenList.Add(gameListener);
			return;
		}

		//otherwise create a new list as a dictionary key
		listenList = new List<IGameListener>();
		listenList.Add(gameListener);
		GameListeners.Add(eventType, listenList);
	}

	//the function to post the event to the game listeners
	public void PostNotification(EventType eventType, Component sender, object param = null)
	{
		//notify all the game listeners of an event
		//the list of game listeners for this event only
		List<IGameListener> listenList = null;

		//if there exists no event, then return
		if(!GameListeners.TryGetValue(eventType, out listenList))
		{
			return;
		}

		//an entry exists so we need to notify the appropriate game listeners
		for(int i = 0; i < listenList.Count; i++)
		{
			if (!listenList[i].Equals(null))
			{
				listenList[i].OnEvent(eventType, sender, param);
			}
		}
	}

	//remove the event from the dictionary including all the listeners of that event
	public void RemoveEvent(EventType eventType)
	{
		GameListeners.Remove(eventType);
	}

	//remove all the redundant entries from the GameListeners dictionary
	public void RemoveRedundancies()
	{
		//create a new dictionary for temporarily holding the active game listeners
		Dictionary<EventType, List<IGameListener>> tempGameListeners = new Dictionary<EventType, List<IGameListener>>();
		
		//go through all the dictionary entries
		foreach(KeyValuePair<EventType, List<IGameListener>> item in GameListeners)
		{
			//go through all the game listeners and remove the null ones
			for(int i = item.Value.Count-1; i >= 0; i--)
			{
				//if the item is null remove it
				if(item.Value[i].Equals(null))
				{
					item.Value.RemoveAt(i);
				}

				//if the item is still in the list then add to the temp dictionary
				if(item.Value.Count > 0)
				{
					tempGameListeners.Add(item.Key, item.Value);
				}
			}

			//replace the game listeners object with the new dictionary
			GameListeners = tempGameListeners;
		}
	}

	#endregion
}
