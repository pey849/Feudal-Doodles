using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputHanderTouch : MonoBehaviour, IInputHandler
{
    private Dictionary<string, ButtonState2> currentFrameButton = new Dictionary<string, ButtonState2>();
    private Dictionary<string, float> currentFrameAxis = new Dictionary<string, float>();
    // Use this for initialization
    void Start()
    {
        currentFrameButton.Add( "Touch", 0 );
        currentFrameAxis.Add( "x", 0 );
        currentFrameAxis.Add( "y", 0 );
    }
    void FixedUpdate()
    {
        //gameObject.GetComponent<GUIText>().text = GetButton("Touch").ToString();

        //foreach (var value in currentFrameButton.Values)
        //{
        //    print(value);
        //}
        //foreach (var value in currentFrameAxis.Values)
        //{
        //    print(value);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.touchCount > 0 )
        {
            if( currentFrameButton["Touch"] == ButtonState2.ButtonDown )
            {
                currentFrameButton["Touch"] = ButtonState2.ButtonHeld;
            }
            else
            {
                currentFrameButton["Touch"] = ButtonState2.ButtonDown;
            }
            currentFrameAxis["x"] = Input.GetTouch( 0 ).position.x;
            currentFrameAxis["y"] = Input.GetTouch( 0 ).position.y;
        }
        else if( ( currentFrameButton["Touch"] == ButtonState2.ButtonDown || currentFrameButton["Touch"] == ButtonState2.ButtonHeld ) && Input.touchCount == 0 )
        {
            currentFrameButton["Touch"] = ButtonState2.ButtonUp;
        }
        else
        {
            currentFrameButton["Touch"] = ButtonState2.ButtonReleased;
        }
    }

    public ButtonState2 GetButton( string name )
    {
        return currentFrameButton[name];
    }

    public float GetAxis( string name )
    {
        return currentFrameAxis[name];
    }
}
