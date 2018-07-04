using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour, IGameListener {

	public AudioSource gameMusic;

	public AudioSource menuMusic;
	
	// Use this for initialization
	void Start () 
	{
		EventManager.Instance.AddGameListener(EventType.GameStarted, this );
		EventManager.Instance.AddGameListener(EventType.ReturnToMenu, this );
    }
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void OnEvent( EventType eventType, Component sender, object param = null )
    {
        switch(eventType) {
		case EventType.GameStarted:
			menuMusic.Stop();
			gameMusic.Play();
			break;
		case EventType.ReturnToMenu:
			gameMusic.Stop();
			menuMusic.Play();
			break;
		}
    }
}
