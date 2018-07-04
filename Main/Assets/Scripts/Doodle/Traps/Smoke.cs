using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour {
    public float despawnTime;

    public bool rising; //is alpha rising or not

    public GameObject me;

    public float alpha = 0;
    SpriteRenderer sprite;
    // Use this for initialization
    void Start () {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        despawnTime = 15.0f;
        alpha = 0;
        rising = true;
        EventManager.Instance.PostNotification(EventType.Smoke, this, null);
    }
	
    public void generateAlpha()
    {
        if(rising)
        {
            alpha += Random.Range(0.003f, 0.008f);
        }
        else
        {
            alpha -= Random.Range(0.003f, 0.008f);
        }

        if(alpha >= 1 && rising == true)
        {
            rising = false;
        }
        else if(alpha <= 0 && rising == false)
        {
            rising = true;
        }
    }

	// Update is called once per frame
	void Update () {
        generateAlpha();
        sprite.color = new Color(255, 255, 255, alpha);
        despawnTime -= Time.deltaTime;
        if (despawnTime <= 0)
        {
            Destroy(gameObject);
            Destroy(me);
        }
    }
}
