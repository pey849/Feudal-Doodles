using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smokeBlockBtn : MonoBehaviour {

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
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().type = "smoke";
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().currentCost = 30;

    }

    public void switchSmoke() {
        Debug.Log("Switched to Smoke");
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().block = trapBlock;
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().type = "smoke";
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().currentCost = 30;
    }
}
