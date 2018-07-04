using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour {
    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.name == "hero 1(Clone)")
        {
            coll.gameObject.GetComponent<PlatformerController>().ice = true;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
