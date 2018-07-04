using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour {

    public float despawnTime;
    public int ID;
    private GameObject GM;

	// Use this for initialization
	void Start () {
        despawnTime = 10.0f;
        GM = GameObject.Find("Game Manager");
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
		if (coll.gameObject != gameObject && (coll.gameObject.name == "SafeZoneRight" || coll.gameObject.name == "SafeZoneLeft"))
        {
            EventManager.Instance.PostNotification(EventType.Explosion, this, null);
            Destroy(gameObject);
        }
        if (coll.gameObject.tag.Equals("Block") && coll.gameObject != this.gameObject)
        {
            EventManager.Instance.PostNotification(EventType.Explosion, this, null);
            Destroy(coll.gameObject);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
		if (coll.gameObject != gameObject && coll.gameObject.name == "hero 1(Clone)")
		{
			PlatformerController player = coll.gameObject.GetComponent<PlatformerController>();
            EventManager.Instance.PostNotification(EventType.Explosion, this, null);
            // Only send death event if I'm the runner that died
            if ( player.IamThisRunner() )
				EventManagerNetworked.Instance.BroadcastEvent(EventType.PlayerDied, new EventManagerArg(player.myTeam));

			Destroy(gameObject);

			return;
		}

        if (coll.gameObject.tag.Equals("Block") && coll.gameObject != this.gameObject)
        {
            Destroy(coll.gameObject);
            Destroy(gameObject);
        }      
    }

    private void FixedUpdate()
    {
        
    }

    // Update is called once per frame
    void Update () {
        despawnTime -= Time.deltaTime;
        if(despawnTime <= 0 )
        {
            Destroy(gameObject);
        }
        

    }
}
