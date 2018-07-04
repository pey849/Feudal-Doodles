using Doodle.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour {
    BoxCollider2D collider2D;
    GameEngine gameEngine;

    private List<Collider2D> colliderList;

    TeamRole owner;

    public void setOwner(TeamRole blockOwner)
    {
        owner = blockOwner;
    }

    //When an object exits our circle collider
    void OnTriggerExit2D(Collider2D collider)
    {
        //if the collider(s) leaves our blast radius (circle collider)
        if (colliderList.Contains(collider))
        {
            //remove it from our blast list so it doesn't get destoryed
            colliderList.Remove(collider);
            Debug.Log("Count: " + colliderList.Count);
            Debug.Log("Item Removed: " + collider.name);
        }
    }

    //When an object enters our circle collider
    void OnTriggerEnter2D(Collider2D collider)
    {
        //print(collider.name);
        if(collider.name =="hero 1(Clone)")
        {
            if (colliderList.Count > 0)
            {
                EventManager.Instance.PostNotification(EventType.Explosion, this, null);
                //destory each game object that is within our circle collider
                foreach (Collider2D c in colliderList)
                {
                    Destroy(c.gameObject);
                    Destroy(this.gameObject);
                }

                //remove all items that we destoryed from our blast list
                colliderList.RemoveRange(0, colliderList.Count);
            }
        }
        //if what we are colliding with is not in our list
        if (!colliderList.Contains(collider))
        {
            Debug.Log(collider.tag);
            //we are only destorying blocks or whatever we decide we are destorying
            if (collider.CompareTag("Block"))
            {
                //add that collider to the blast list
                colliderList.Add(collider);
                Debug.Log("Count: " + colliderList.Count);
                Debug.Log("Item Added: " + collider.name);
            }
        }

    }

    void Awake()
    {
        collider2D = this.GetComponent<BoxCollider2D>();
        //initialize the blast list
        colliderList = new List<Collider2D>();
        //gameEngine = GameObject.Find("Game Manager").GetComponent<GameEngine>(); //Uncomment this
    }

    // Update is called once per frame
    void Update ()
    {
        //print(colliderList.Count);
	}
}
