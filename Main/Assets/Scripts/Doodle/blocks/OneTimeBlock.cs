using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.Networking.Packets;

public class OneTimeBlock : MonoBehaviour {

    void OnCollisionExit2D(Collision2D coll)
    {
        if(coll.gameObject.name == "hero 1(Clone)")
        {
            GameState.Block b2 = new GameState.Block();
            b2.position = new Vector3(transform.position.x, transform.position.y, 6f);
            b2.type = GetComponent<Block>().type;
            b2.role = GetComponent<Block>().role;

            GameObject.Find("Game Manager").GetComponent<GameEngine>().sendBlockUpdate(b2, false);
            Destroy(gameObject);
        }
    }

        // Use this for initialization
        void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
