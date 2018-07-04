using UnityEngine;
using System.Collections;
using Doodle.Game;

public class CameraBehaviour : MonoBehaviour, IGameListener
{
    //the parallax scrolling backgrounds attached to the Camera
    private Component[] parallaxBackgrounds;

    //the target to follow, aka the Player
    public Transform target;

    //the bounds for the camera, so that the x and y of the camera won't scroll past these points
    public float cameraLeftBoundX;
    public float cameraRightBoundX;
    public float cameraTopBoundY;
    public float cameraBottomBoundY;

    //Current X and Y position of the camera
    private float currCameraPositionX;
    private float currCameraPositionY;

	//the offset for moving the textures
	private Vector2 deltaOffset;

	private bool reset;

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        this.parallaxBackgrounds = GetComponentsInChildren<ParallaxScrolling>();

		//subscribe to the player died event
		EventManager.Instance.AddGameListener(EventType.PlayerDied, this);
		
		reset = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
		if(reset)
		{
			transform.position = new Vector3(this.target.position.x, this.target.position.y, transform.position.z);
			ResetTextureOffsets();
			reset = false;
		} 
		else 
		{
			//the change in texture offset needed, calculated based on the player's movement
	        deltaOffset = new Vector2((target.position.x - transform.position.x) / 100, 0.0f);
	        float yDelta = ((transform.position.y) - target.position.y) / 100;
	
	        //calculate camera's x and y positions
			deltaOffset.y = yDelta;
	        this.assignCameraPositions();
	        //set the camera's new position
	        transform.position = new Vector3(this.currCameraPositionX, this.currCameraPositionY, transform.position.z); //ySmoothPosition
	
	        //update each parallax background attached to the camera
	        //passing in the deltaOffset to determine additional scroll offset
	        foreach (ParallaxScrolling bg in this.parallaxBackgrounds)
	        {
	            bg.UpdateBackground(deltaOffset);
	        }
		}  
    }

	//Reassigns the parallax backgrounds in case some are dynamically loaded in
    public void ReassignBackgrounds()
    {
        this.parallaxBackgrounds = GetComponentsInChildren<ParallaxScrolling>();
    }

	//Resets the texture offsets for all backgrounds in parallaxBackgrounds to their original offsets
	public void ResetTextureOffsets()
	{
		foreach (ParallaxScrolling bg in this.parallaxBackgrounds)
        {
            bg.ResetTextureOffset();
        }
	}

    public void assignCameraPositions()
    {
        //set the camera position to follow the target regardless
        this.currCameraPositionX = this.target.position.x;
        this.currCameraPositionY = this.target.position.y;

        //run checks to just auto correct if we hit the vertical and horizontal bounds
        //check to see the target has moved past the x bounds
        if (target.position.x < this.cameraLeftBoundX)
        {
            //lock camera x to left boundary
            currCameraPositionX = this.cameraLeftBoundX;
			deltaOffset = new Vector2(0f, deltaOffset.y);
        }
        else if (target.position.x > this.cameraRightBoundX)
        {
            //lock camera x to right boundary
            currCameraPositionX = this.cameraRightBoundX;
			deltaOffset = new Vector2(0f, deltaOffset.y);
        }

        if (target.position.y < this.cameraBottomBoundY)
        {
            //lock camera y to bottom boundary
            currCameraPositionY = this.cameraBottomBoundY;
			deltaOffset = new Vector2(deltaOffset.x, 0f);
        }
        else if (target.position.y > this.cameraTopBoundY)
        {
            //lock camera y to top boundary
            currCameraPositionY = this.cameraTopBoundY;
			deltaOffset = new Vector2(deltaOffset.x, 0f);
        }
    }

	//handles the events subscribed to
	public void OnEvent(EventType eventType, Component sender, object arg = null)
	{
		switch(eventType)
		{
			case EventType.PlayerDied:
				reset = true;
			break;
		}
	}

	public void SetupRoles(TeamRole role)
	{
		foreach (ParallaxScrolling bg in this.parallaxBackgrounds)
        {
            bg.SetMyRole(role);
        }
	}
}
