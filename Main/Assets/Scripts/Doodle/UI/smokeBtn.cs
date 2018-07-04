using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smokeBtn : MonoBehaviour
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

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().block = normalBlock;
            GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>().type = "smoke";
        }
    }
}
