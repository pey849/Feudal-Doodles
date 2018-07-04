using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonState2 {ButtonUp, ButtonDown, ButtonHeld, ButtonReleased};

interface IInputHandler{
    ButtonState2 GetButton(string name);
    float GetAxis(string name);
}


public class InputHandlerGamepad : MonoBehaviour, IInputHandler {

    private Dictionary<string, ButtonState2> currentFrameButton = new Dictionary<string, ButtonState2>();
    private Dictionary<string, float> currentFrameAxis = new Dictionary<string, float>();


    // Use this for initialization
    void Start () {      
        currentFrameButton.Add("Jump", ButtonState2.ButtonDown);
        currentFrameAxis.Add("MoveLeft", 0F);
        currentFrameAxis.Add("MoveRight", 0F);
    }
	
	//void FixedUpdate()
 //   {
 //       foreach (var value in currentFrameButton.Values)
 //       {
 //           print(value);
 //       }
 //       foreach (var value in currentFrameAxis.Values)
 //       {
 //           print(value);
 //       }
 //   }
    
    // Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Jump"))
        {
            currentFrameButton["Jump"] = ButtonState2.ButtonDown;
        }
        else if (Input.GetButton("Jump"))
        {
            currentFrameButton["Jump"] = ButtonState2.ButtonHeld;
        }
        else if(Input.GetButtonUp("Jump"))
        {
            currentFrameButton["Jump"] = ButtonState2.ButtonReleased;
        }
        else
        {
            currentFrameButton["Jump"] = ButtonState2.ButtonUp;
        }


        if (Input.GetAxis("LeftStick") > 0)
        {
            currentFrameAxis["MoveRight"] = Input.GetAxis("LeftStick");
        }
        else
        {
            currentFrameAxis["MoveRight"] = Input.GetAxis("LeftStick");
        }

        if (Input.GetAxis("LeftStick") < 0)
        {
            currentFrameAxis["MoveLeft"] = Input.GetAxis("LeftStick");
        }
        else
        {
            currentFrameAxis["MoveLeft"] = Input.GetAxis("LeftStick");
        }
    }

    public ButtonState2 GetButton(string name)
    {
        return currentFrameButton[name];
    }


    public float GetAxis(string name)
    {
        return currentFrameAxis[name];
    }
}
