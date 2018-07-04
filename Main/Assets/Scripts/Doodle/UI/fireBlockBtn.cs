using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireBlockBtn : MonoBehaviour {

    public GameObject normalBlock;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseOver()
    {
        Debug.Log("FIRE");
        print("test");
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().block = normalBlock;
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().type = "fire";
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().currentCost = 40;

    }
}
