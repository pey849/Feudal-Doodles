using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.Networking.Packets;
using Doodle.Game;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuilderPlatformer : MonoBehaviour
{

    public enum TouchState
    {
        Ready,
        BlockPlacement,
        ScreenSwipe,
        TrapPlacement,
        BlockChoosing,
        Decision
    }

    public Vector2 startPos;
    public Vector2 direction;
    public bool directionChosen;
    public GameObject block;
    public string type;
    public GameObject Rend;
    public float scrollSpeed;
	public float mouseScrollSpeed;
    public LayerMask layerMask;

    TouchState state = TouchState.Ready;
    private int currTouchID, prevTouchID;
    private bool didSwipe = false;

    public float cameraLeftBoundX;
    public float cameraRightBoundX;
    public float cameraTopBoundY;
    public float cameraBottomBoundY;


    float builderPositionTanslationX;
    float builderPositionTanslationY;

    bool debugMessage = false;
    bool debugTouchMessages = false;

    public Team myTeam;
    private GameObject gameManager;

    Economy myEcon;
    
    int myCurrentGoldAmount;
    public int warningGoldAmount;
    Text displayOwnGold;

    public int currentCost;
    public bool enableMouseClick = true;

    // Use this for initialization
    void Start()
    {
		//this.enableMouseClick = false;
        Rend = GameObject.Find("Renderer");
        type = "normal";
        //grab the economy system
        myEcon = GameObject.Find("Economy(Clone)").GetComponent<Economy>();
        displayOwnGold = GameObject.Find("GoldCount").GetComponent<Text>();
        currentCost = 40;
        gameManager = GameObject.Find("Game Manager");


		#if UNITY_STANDALONE
		Debug.Log("Windows Standalone");
		enableMouseClick = true;
		#endif

		#if UNITY_ANDROID
		Debug.Log("Android");
			enableMouseClick = false;
		#endif

		#if UNITY_EDITOR
		Debug.Log("Windows editor");
		enableMouseClick = true;
		#endif
    }

    // Update is called once per frame
    void Update()
    {
        //check who's gold it needs to grab
        if (myTeam.Equals(Team.Purple))
        {
            myCurrentGoldAmount = myEcon.getLeftGold();
            
        }
        else if (myTeam.Equals(Team.Yellow))
        {
            myCurrentGoldAmount = myEcon.getRightGold();
        }

        //update the visual gold amount
        displayOwnGold.text = string.Format(string.Format("Gold: ${0}", myCurrentGoldAmount));
        if (myCurrentGoldAmount < warningGoldAmount)
        {
            displayOwnGold.color = Color.red;
        }
        else {
            displayOwnGold.color = Color.white;
        }

        //Debug.Log("State: " + state);
		if (enableMouseClick) {
			if (Input.GetMouseButtonDown (0)) {
				mouseClickTest ();
			}
			else if(Input.GetMouseButton(1)){

				//get the vector movement of when right click is down and moving
				Vector2 mousePos = new Vector2 (Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"));
				this.setBuilderPosition(mousePos);
				transform.Translate (-this.builderPositionTanslationX * mouseScrollSpeed, -this.builderPositionTanslationY * mouseScrollSpeed, 0);

			}

		} else if (!enableMouseClick) {
			//detect if we got any touches
			if (Input.touchCount > 0) {
				//get the touch
				Touch touch = Input.GetTouch (0);
				//if the count is 2, we are going to do swiping
				if (Input.touchCount == 2) {
					didSwipe = true;
					if (debugTouchMessages) {
						Debug.Log ("Going into swipe state");
					}

				}

				//state machine
				switch (state) {
				//ready state
				case TouchState.Ready:
					switch (touch.phase) {
					case TouchPhase.Began:
						if (isTouchingBuilderUITouch (touch)) {
							Debug.Log ("Doing Nothing");
						}
	                            //if we have two fingers down
	                            else if (didSwipe) {
							if (debugTouchMessages) {
								Debug.Log ("Going to swipe");
							}
							//go to swipe state
							state = TouchState.ScreenSwipe;
						}
	                            //two touches may have not happened right away
	                            else {
							if (debugTouchMessages) {
								Debug.Log ("Detected 1 finger. going to make decision");
							}
							//go to decision state to make sure what state we need to be in
							state = TouchState.Decision;
							prevTouchID = currTouchID;
						}

						break;
					}
					break;
				case TouchState.Decision:
					switch (touch.phase) {
					case TouchPhase.Moved:
	                            //we have two fingers down and are moving
						if (didSwipe) {
							if (debugTouchMessages) {
								Debug.Log ("Detected 2 fingers. going to swipe");
							}
							//we know its a two finger touch since the touch id changed
							state = TouchState.ScreenSwipe;
						}
						break;

					//else we only had 1 finger down and lifted, meaning we are placing a block
					case TouchPhase.Ended:
						if (debugTouchMessages) {
							Debug.Log ("Detected 1 fingers. going to block placement");
						}
	                            //the touch never changed so its just one finger
						doTouchPlacement (touch);
						didSwipe = false;
						state = TouchState.Ready;
						break;
					}
					break;

				//Swipe state
				case TouchState.ScreenSwipe:
					switch (touch.phase) {
					case TouchPhase.Moved:
						this.setBuilderPosition (Input.GetTouch (0).deltaPosition);
						transform.Translate (-this.builderPositionTanslationX * scrollSpeed, -this.builderPositionTanslationY * scrollSpeed, 0);
						break;
					//ended swipe, go back to ready state
					case TouchPhase.Ended:
						didSwipe = false;
						state = TouchState.Ready;
						break;
					}
					break;

				//dummy states, future implementation possibly
				case TouchState.TrapPlacement:
					break;
				case TouchState.BlockChoosing:
					break;
				}
			}
		}

    }

    public void setBuilderPosition(Vector2 builderPositionTanslation)
    {

        this.builderPositionTanslationX = builderPositionTanslation.x;
        this.builderPositionTanslationY = builderPositionTanslation.y;

        //Debug.Log("X Pos: " + this.builderPositionTanslationX);
        //Debug.Log("Y Pos: "+ this.builderPositionTanslationY);

        //swiping up is positive = camera is moving down
        //swiping down is negative = camera is move up

        //if we are at the bottom bounds and we are swiping up OR we are at the top bounds and swiping down
        //don't move the builder's position
        if ((transform.position.y <= this.cameraBottomBoundY && this.builderPositionTanslationY > 0)
            || (transform.position.y >= this.cameraTopBoundY && this.builderPositionTanslationY < 0))
        {

            //no y movement is given
            this.builderPositionTanslationY = 0;
        }

        //swiping left is negative = camera is moving right
        //swiping right is positive = camera is moving left

        //if we are at the bottom bounds and we are swiping up OR we are at the top bounds and swiping down
        //don't move the builder's position
        if (((transform.position.x <= this.cameraLeftBoundX && this.builderPositionTanslationX > 0)
            || (transform.position.x >= this.cameraRightBoundX && this.builderPositionTanslationX < 0)))
        {
            //no x movement is given
            this.builderPositionTanslationX = 0;
        }



    }

    //Testing Purpose: Using mouse clicks to simulate touch. can disable later
    public void mouseClickTest()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 20f, layerMask);

        int layer_mask = LayerMask.GetMask("Walls");
        RaycastHit2D castForBlock = Physics2D.Raycast(pos, Vector2.zero, 20f, layer_mask);

        //if we are not clicking on UI buttons, proceed
        if (!isTouchingBuilderUIClick())
        {
            //Did we hit a map location that blocks can't be placed
            if (hit)
            {
                if (isTouchingCastle(hit))
                {
                    //Do nothing as either Yellow or Purple is touching their opponent's safe zone
                    if (debugMessage) { Debug.Log("off limits"); }
                }
                //are we touching an opponent's safezone
                else if (isInOpponentSafeZone(hit))
                {
                    //Do nothing as either Yellow or Purple is touching their opponent's safe zone
                    //Debug.Log("It works");
                }
                //else we are placing a block in player's own safe zone or in no man's land
                else
                {
                    if (currentCost <= myCurrentGoldAmount && !castForBlock)
                    {
                        Debug.Log(currentCost + "and myCurrentGold: " + myCurrentGoldAmount);
                        placeBlockClick();
                    }
                }

            }
            //place block in no-man's land
            else
            {
                if (currentCost <= myCurrentGoldAmount && !castForBlock)
                {
                    Debug.Log(currentCost + "and myCurrentGold: " + myCurrentGoldAmount);
                    placeBlockClick();
                }
            }
        }
    }

    public void placeBlockClick()
    {
        //decrement the gold
        if (myTeam.Equals(Team.Purple))
        {
            myEcon.reduceLeftGold(currentCost);
        }
        else if (myTeam.Equals(Team.Yellow))
        {
            myEcon.reduceRightGold(currentCost);
        }

        startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        directionChosen = false;

        if (debugMessage) { Debug.Log("Began mouse at: " + startPos); }

        GameState.Block b2 = new GameState.Block();
        b2.position = new Vector3(startPos.x, startPos.y, 6f);
        b2.type = type;
        b2.role = gameManager.GetComponent<GameEngine>().getTeamRole();

        gameManager.GetComponent<GameEngine>().sendBlockUpdate(b2, true);

        // Update statistics of number of blocks placed by me.
        EventManagerNetworked.Instance.BroadcastEvent(EventType.BlockPlaced, new EventManagerArg(myTeam));
        
    }

    public void doTouchPlacement(Touch touch)
    {
        startPos = Camera.main.ScreenToWorldPoint(touch.position);
        //detect players and castles
        Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 20f, layerMask);

        int layer_mask = LayerMask.GetMask("Walls");
        RaycastHit2D castForBlock = Physics2D.Raycast(pos, Vector2.zero, 20f, layer_mask);

        //if we are not touching UI buttons, proceed
        if (isTouchingBuilderUIClick() == false)
        {
           
            if (hit)
            {
                //are we touching an opponent's safezone
                if (isInOpponentSafeZone(hit))
                {
                    //Do nothing as either Yellow or Purple is touching their opponent's safe zone
                    if (debugMessage) { Debug.Log("It works"); }
                }
                else if (isTouchingCastle(hit)) {
                    //Do nothing as either Yellow or Purple is touching their opponent's safe zone
                    if (debugMessage) { Debug.Log("off limits"); }
                }
                //else we are placing a block in player's own safe zone or in no man's land
                else
                {
                    if (currentCost <= myCurrentGoldAmount && !castForBlock)
                    {
                        placeBlockTouch();
                    }
                }
            }
            else
            {
                //place block in no-man's land
                if (currentCost <= myCurrentGoldAmount && !castForBlock)
                {
                    placeBlockTouch();
                }
            }
        }
    }

    public void placeBlockTouch()
    {
        //decrement the gold
        if (myTeam.Equals(Team.Purple))
        {
            myEcon.reduceLeftGold(currentCost);
        }
        else if (myTeam.Equals(Team.Yellow)) {
            myEcon.reduceRightGold(currentCost);
        }

        directionChosen = true;
        if (debugMessage) { Debug.Log("End Touch"); }
        GameState.Block b2 = new GameState.Block();
        b2.position = new Vector3(startPos.x, startPos.y, 6f);
        b2.type = type;
        b2.role = gameManager.GetComponent<GameEngine>().getTeamRole();

        gameManager.GetComponent<GameEngine>().sendBlockUpdate(b2, true);

		// Update statistics of number of blocks placed by me.
		EventManagerNetworked.Instance.BroadcastEvent(EventType.BlockPlaced, new EventManagerArg(myTeam));
    }

    //are we touching the oppoenent's safezone and trying to put non-one time blocks in
    public bool isInOpponentSafeZone(RaycastHit2D hit)
    {
        return ((hit.transform.gameObject.layer == LayerMask.NameToLayer("SafeZoneLeft")
            || hit.transform.gameObject.layer == LayerMask.NameToLayer("SafeZoneRight")) 
            && (type != "onetime" && type != "normal"));
    }

    //are we touching the castle
    public bool isTouchingCastle(RaycastHit2D hit)
    {
        return (hit.transform.gameObject.layer == LayerMask.NameToLayer("Castles"));
    }

    //are we clicking on the UI buttons
    public bool isTouchingBuilderUIClick() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    //are we touching the UI buttons or no
    public bool isTouchingBuilderUITouch(Touch t)
    {
        return EventSystem.current.IsPointerOverGameObject(t.fingerId);
    }


}
