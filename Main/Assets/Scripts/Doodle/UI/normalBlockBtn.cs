﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class normalBlockBtn : MonoBehaviour
{

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

        print("test");
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().block = normalBlock;
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().type = "normal";
        GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().currentCost = 10;

    }
}
