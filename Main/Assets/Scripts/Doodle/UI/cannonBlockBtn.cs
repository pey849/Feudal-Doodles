using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannonBlockBtn : MonoBehaviour {

    public GameObject trapBlock;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMouseOver()
    {

        print("test");
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().block = trapBlock;
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().type = "cannon";
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().currentCost = 5;

    }

    public void switchCannon()
    {
        Debug.Log("Switched to cannon");
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().block = trapBlock;
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().type = "cannon";
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().currentCost = 5;
    }
}
