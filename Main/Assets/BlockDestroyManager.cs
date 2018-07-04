using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDestroyManager : MonoBehaviour, IGameListener {

	public GameObject blockDestroy;

	// Use this for initialization
	void Start () {
		EventManager.Instance.AddGameListener(EventType.BlockDestroyed, this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnEvent( EventType eventType, Component sender, object param = null ) {
		Instantiate(blockDestroy, (Vector3)param, new Quaternion());
	}
}
