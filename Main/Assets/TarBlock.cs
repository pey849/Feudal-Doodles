using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarBlock : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name == "hero 1(Clone)")
        {
            coll.gameObject.GetComponent<PlatformerController>().tar = true;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
