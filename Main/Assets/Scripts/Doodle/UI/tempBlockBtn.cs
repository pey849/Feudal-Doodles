using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempBlockBtn : MonoBehaviour {

    public GameObject normalBlock;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMouseOver()
    {

        print("test");
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().block = normalBlock;
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().type = "temp";
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().currentCost = 5;

    }

    public void switchTemp()
    {
        Debug.Log("Switched to Temp");
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().block = normalBlock;
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().type = "temp";
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().currentCost = 5;
    }
}
