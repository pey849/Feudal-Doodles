using System.Collections;
using System.Collections.Generic;
using Doodle.Game;
using UnityEngine;

public class Block : MonoBehaviour {

    public string type;
    public TeamRole role;

    public float delayFromServer = 0;
    protected float fadeMaxTime;
	protected float fadeCurTime;
	protected float fadeCurAlpha;
	protected Color blockColor;

	protected GameEngine engine;

    // Use this for initialization
    void Start ()
	{
        // Disble collider
        GetComponent<BoxCollider2D>().enabled = false;

        // Make completely transparent
		fadeCurAlpha = 0.0f;
        blockColor = gameObject.GetComponent<SpriteRenderer>().color;
		blockColor.a = fadeCurAlpha;
        gameObject.GetComponent<SpriteRenderer>().color = blockColor;

		fadeMaxTime = System.Math.Abs(1.0f - delayFromServer);
		if (fadeMaxTime > 0.5f)
			fadeMaxTime = 0.5f;
		
        fadeCurTime = 0.0f;

		this.engine = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameEngine>();
    }

	// Update is called once per frame
	void Update ()
    {
		// Increase alpha a bit if it's not 1.0
		if (fadeCurAlpha < 1.0)
		{
			fadeCurAlpha = fadeCurTime / fadeMaxTime;
			gameObject.GetComponent<SpriteRenderer>().color = new Color(blockColor.r, blockColor.g, blockColor.b, fadeCurAlpha);
			fadeCurTime += Time.deltaTime;
		}

		// Enable collider if faded in enough (+33%)
        if (fadeCurTime >= fadeMaxTime/3)
            GetComponent<BoxCollider2D>().enabled = true;
    }

	void OnDestroy() 
	{
		Vector3 position = transform.position;

		if (this.engine.isRunning)
			EventManager.Instance.PostNotification(EventType.BlockDestroyed, null, position);
	}
}
