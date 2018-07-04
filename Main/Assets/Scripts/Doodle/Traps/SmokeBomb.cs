using Doodle.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBomb : MonoBehaviour
{

    BoxCollider2D collider2D;
    public GameObject smoke;
    GameEngine gameEngine;

    TeamRole owner;

    private bool once = false;

    public void setOwner(TeamRole blockOwner)
    {
        owner = blockOwner;
    }

    // Use this for initialization
    void Awake()
    {
        collider2D = this.GetComponent<BoxCollider2D>();
        //gameEngine = GameObject.Find("Game Manager").GetComponent<GameEngine>(); //Uncomment this
    }

    void Start()
    {
        Vector3 startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 touchPos = new Vector2(startPos.x, startPos.y);
        var s = Instantiate(smoke, this.gameObject.transform.position, this.gameObject.transform.rotation);
        s.GetComponent<Smoke>().me = gameObject;
    }

    private void FixedUpdate()
    {
        Color blockColor = gameObject.GetComponent<SpriteRenderer>().color;
        blockColor.a = 0f;
        gameObject.GetComponent<SpriteRenderer>().color = blockColor;
        Destroy(gameObject);
    }

    // Update is called once per frame
    //void Update()
    //{
    //    //if (Input.touchCount > 0  && gameEngine.getTeamRole() == owner) //Uncomment this later
    //    //if (Input.GetMouseButtonDown(0)) //Change to touch, delete this line
    //    //{
    //        Vector3 startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Change to touch, delete this line
    //      //Vector3 startPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position); //Uncomment this later
    //        Vector2 touchPos = new Vector2(startPos.x, startPos.y);
    //    //if (collider2D == Physics2D.OverlapPoint(touchPos))
    //    //{
    //    //if (collider2D == Physics2D.OverlapPoint(touchPos))
    //    //{
    //    if (once == false) {
    //        var s = Instantiate(smoke, this.gameObject.transform.position, this.gameObject.transform.rotation);
    //        s.GetComponent<Smoke>().me = gameObject;
    //        once = true;
    //    }
    //                //Destroy(gameObject);
    //            //}
    //        //}
    //    //}
    //}
}
