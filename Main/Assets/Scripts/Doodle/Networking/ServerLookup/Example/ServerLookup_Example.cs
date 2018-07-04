using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerLookup_Example : MonoBehaviour 
{
	protected IServerLookup mLookup;
	public InputField mRoomCodeField;

	void Start ()
	{
		mLookup = this.gameObject.GetComponent<IServerLookup>();	
	}

	void Update () 
	{
		
	}
		
	public void OnClick_GetServerIP()
	{
		mLookup.GetServerIP(mRoomCodeField.text);
	}
}
