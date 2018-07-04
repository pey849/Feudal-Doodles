using UnityEngine;
using System.Collections;
using Doodle.Game;

public class ParallaxScrolling : MonoBehaviour, IGameListener
{

    /* 
     The different parallax scroll types.
     AutoScroll: background scrolls automatically without taking into account player movement
     AutoMovement: background scrolls automatically while taking player movement into account
     Stationary: background does not scroll at all
     PlayerMovement: background scrolls only based on player movement 
    */
    public enum ScrollType { AutoScroll, AutoMovement, Stationary, PlayerMovement }

    //the texture renderer of the background
    private Material textureMaterial;

    //the horizontal scroll speed of the background
    public float xScrollSpeed;

    //the vertical scroll speed of the background
    public float yScrollSpeed;

    //the multiplier value for how much the camera's movement affects the x scrolling
    //Value of 1 is normal scroll, greater than 1 is faster scroll, less than 1 is slower scroll
    public float xMovementMultiplier;

    //the multiplier value for how much the camera's movement affects the y scrolling
    //Value of 1 is normal scroll, greater than 1 is faster scroll, less than 1 is slower scroll
    public float yMovementMultiplier;

    //does the background scroll automatically?
    public ScrollType scrollType;

	//the texture offsets
	private float xTextureOffset;
	private float yTextureOffset;

	//the original texture offsets, used for reseting
	private float xStartOffset;
	private float yStartOffset;

	private TeamRole myRole;

    // Use this for initialization
    void Start ()
    {
        //set the material
        this.textureMaterial = GetComponent<MeshRenderer>().material;
		this.xStartOffset = this.textureMaterial.mainTextureOffset.x;
		this.yStartOffset = this.textureMaterial.mainTextureOffset.y;
		SetupOffset();
		this.xTextureOffset = this.textureMaterial.mainTextureOffset.x;
		this.yTextureOffset = this.textureMaterial.mainTextureOffset.y;
		
		//subscribe to the player died event
		EventManager.Instance.AddGameListener(EventType.GameStarted, this);
		
    }

    public void UpdateBackground(Vector2 deltaOffset)
    {
        switch (scrollType)
        {
            //if auto scroll is enabled, scroll the texture automatically scrolls
            //without player movement taken into account
            case ScrollType.AutoScroll:
                this.textureMaterial.mainTextureOffset = new Vector2(this.textureMaterial.mainTextureOffset.x + this.xScrollSpeed,
                    this.textureMaterial.mainTextureOffset.y + this.yScrollSpeed);
                break;
            //if auto movement is enabled, scroll the texture automatically while 
            //taking into account the player movement
            case ScrollType.AutoMovement:
                this.textureMaterial.mainTextureOffset = new Vector2(this.textureMaterial.mainTextureOffset.x + this.xScrollSpeed + (this.xMovementMultiplier * deltaOffset.x),
                     this.textureMaterial.mainTextureOffset.y + this.yScrollSpeed - (this.yMovementMultiplier * (3 * deltaOffset.y)));
                break;
            //if stationary is enabled, do not scroll the backgrounds at all
            case ScrollType.Stationary:
                //do nothing, texture offset is good and no scrolling is needed
                break;
            //if player movement is enabled, scroll the texture only by player movement
            case ScrollType.PlayerMovement:
                this.textureMaterial.mainTextureOffset = new Vector2(this.textureMaterial.mainTextureOffset.x + (this.xMovementMultiplier * deltaOffset.x),
                    this.textureMaterial.mainTextureOffset.y + (this.yMovementMultiplier * (-1 * deltaOffset.y)));
                break;

            default:
                //do nothing
                break;
        }
    }

	public void ResetTextureOffset()
	{
		this.textureMaterial.mainTextureOffset = new Vector2(this.xTextureOffset, this.yTextureOffset);
	}

	//handles the events subscribed to
	public void OnEvent(EventType eventType, Component sender, object arg = null)
	{
		switch(eventType)
		{
			case EventType.GameStarted:
				SetupOffset();
				this.xTextureOffset = this.textureMaterial.mainTextureOffset.x;
				this.yTextureOffset = this.textureMaterial.mainTextureOffset.y;
                ResetTextureOffset();
			break;
		}
	}

	public void SetupOffset()
	{	
		//accommodate the fact that the offsets will all be different for each role
		//as they spawn in different spots
		if(myRole == TeamRole.PurpleBuilder)
		{
			//just adjust the y offset
            this.textureMaterial.mainTextureOffset = new Vector2(this.xStartOffset,
                    this.yStartOffset);// + (this.yMovementMultiplier * ((-4f)/100f)));
		}
		else if(myRole == TeamRole.YellowRunner)
		{
			//just adjust the x offset
			this.textureMaterial.mainTextureOffset = new Vector2(this.xStartOffset + (this.xMovementMultiplier * ((30f)/100f)),
                    this.yStartOffset);// + (this.yMovementMultiplier * ((-4f)/100f)));
		}
		else if(myRole == TeamRole.YellowBuilder)
		{
			//adjust both the x and the y offset
			this.textureMaterial.mainTextureOffset = new Vector2(this.xStartOffset + (this.xMovementMultiplier * ((30f)/100f)),
                    this.yStartOffset);// + (this.yMovementMultiplier * ((-4f)/100f)));
		}
		//you have to be the purple runner
		else
		{
			this.textureMaterial.mainTextureOffset = new Vector2(this.xStartOffset, this.yStartOffset);// + (this.yMovementMultiplier * ((-4f)/100f)));
		}
	}

	public void SetMyRole(TeamRole role)
	{
		this.myRole = role;
	}
}
