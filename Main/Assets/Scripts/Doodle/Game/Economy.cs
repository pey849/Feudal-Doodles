using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Economy : MonoBehaviour
{
    //values for teams in-game gold
    public int leftGold;
    public int rightGold;

    //Amount of gold to give per cycle
    public int goldPerTurn;

    public int captureReward;

    public void incrementGold()
    {
        leftGold += goldPerTurn;
        rightGold += goldPerTurn;
    }

    //Getter for left team's gold
    public int getLeftGold()
    {
        return leftGold;
    }

    //Getter for right team's gold
    public int getRightGold()
    {
        return rightGold;
    }

    //function to check if team has enough gold to make purchase, returns true if they can, false otherwise.
    //side = 0 for left side, side = 1 for right side.
    public bool canAffordBlock(int side, int blockPrice)
    {
        if (side == 0)
        {
            if (blockPrice <= leftGold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (side == 1)
        {
            if (blockPrice <= rightGold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //Public function to decrease gold when purchase is made
    public void reduceLeftGold(int decrement)
    {
        leftGold -= decrement;
    }

    //Public function to decrease gold when purchase is made
    public void reduceRightGold(int decrement)
    {
        rightGold -= decrement;
    }

    public void incrementLeft(int goldAmount) {

        leftGold += goldAmount;

    }

    public void incrementRight(int goldAmount)
    {
        rightGold += goldAmount;
    }


    //Public function to set gold of left team
    public void setLeftGold(int newVal)
    {
        leftGold = newVal;
    }

    //Public function to set gold of right team
    public void setRightGold(int newVal)
    {
        rightGold = newVal;
    }

    // Use this for initialization
    void Start () {
        //leftGold = 0;
        //rightGold = 0;

        //goldPerTurn = 50;
    }
	
	// Update is called once per frame
	void Update () {

        //var leftGoldText = GameObject.Find("LeftGold").GetComponent<Text>();
        //var rightGoldText = GameObject.Find("RightGold").GetComponent<Text>();

        //Update team gold UI text
        //leftGoldText.text = string.Format("Gold: {0}", leftGold);
        //rightGoldText.text = string.Format("Gold: {0}", rightGold);
    }
}
