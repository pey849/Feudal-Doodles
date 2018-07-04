using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mineBlockBtn : MonoBehaviour {

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
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().type = "mine";
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().currentCost = 1;

    }

    public void switchMine()
    {
        Debug.Log("Switched to mine");
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().block = trapBlock;
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().type = "mine";
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().currentCost = 1;
    }
}
