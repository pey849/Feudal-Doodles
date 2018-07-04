using UnityEngine;
using UnityEngine.UI;


//a class of functions for buttons to be used by the builder
public class BuilderUI_Buttons : MonoBehaviour {

    bool debugLog = false;

    //a color to signify which button is selected
    public Color selectedColor;
    //if no longer selected the button default over lay color is whites
    private Color prevSelectedColor = Color.white;
    //keep track of what button was previously and is currently selected
    private Image prevSelected;
    private Image currSelected;

    //reference to the builder to avoid expensive lookups
    private BuilderPlatformer builderObject = null;

    private string whichBlockSet = "set_1";
    
    public GameObject[] allBlockPrefabs;
    public GameObject[] allButtons;
    public Text[] allButtonText;
    public Image[] allButtonImages;
    public int[] blockPrice;

    void Awake() {

        //default selection
        prevSelected = allButtonImages[0].GetComponent<Image>();
        currSelected = allButtonImages[0].GetComponent<Image>();
        currSelected.color = selectedColor;

        //set up the block buttons on awake - dynamically setup the images, text
        for (int i = 0; i < allButtons.Length; i++)
        {
            allButtonText[i].GetComponent<Text>().text = "\n$" + blockPrice[i];
            allButtonImages[i].GetComponent<Image>().sprite = allBlockPrefabs[i].GetComponent<SpriteRenderer>().sprite;
        }

    }

    void Start()
    {
        //find a reference to the builder
        try {
            if (builderObject == null)
            {
                if (debugLog) { Debug.Log("HI"); }
                builderObject = GameObject.Find("Builder(Clone)").GetComponent<BuilderPlatformer>();
                //inital setup
                switchToNormalBloc();
                turnOffButtons(whichBlockSet);
            }

        }
        //the ui gets spawned but no builder for the platformer to see so a null reference needs to be caught
        //error is harmless
        catch (System.Exception e) {
            if (debugLog)
            { Debug.Log("Caught some shit"); }
        }

    }

    //each function is attached to a button in the inspector
    //used for switching block types and to notify the builder what blocks
    //to give to the renderer and how much they cost

    //also switches to highlight the button they are on

    /******************* Pair 0 and Pair 1 ************************/
    public void switchToNormalBloc()
    {

        if (debugLog)
        { Debug.Log("Normal"); }
        builderObject.block = allBlockPrefabs[0];
        builderObject.type = "normal";
        builderObject.currentCost = blockPrice[0];
        switchSelectionColor(allButtons[0].GetComponent<Image>());

    }

    public void switchToIceBlock()
    {

        if (debugLog)
        { print("Ice"); }
        builderObject.block = allBlockPrefabs[1];
        builderObject.type = "ice";
        builderObject.currentCost = blockPrice[1];

        switchSelectionColor(allButtons[1].GetComponent<Image>());

    }


    /******************* Pair 2 and Pair 3 ************************/

    public void switchToOneTimeBlock()
    {
        if (debugLog)
        { Debug.Log("One Time"); }
        builderObject.block = allBlockPrefabs[2];
        builderObject.type = "onetime";
        builderObject.currentCost = blockPrice[2];

        switchSelectionColor(allButtons[2].GetComponent<Image>());
    }

    public void switchToJumpPadBlock()
    {
        if (debugLog)
        { Debug.Log("Jump Pad"); }
        builderObject.block = allBlockPrefabs[3];
        builderObject.type = "jumppad";
        builderObject.currentCost = blockPrice[3];

        switchSelectionColor(allButtons[3].GetComponent<Image>());
    }



    /******************* Pair 4 and Pair 5 ************************/
    public void switchToCannonTrap()
    {
        if (debugLog)
        { Debug.Log("Switched to cannon"); }
        builderObject.block = allBlockPrefabs[4];
        builderObject.type = "cannon";
        builderObject.currentCost = blockPrice[4];

        switchSelectionColor(allButtons[4].GetComponent<Image>());
    }


    public void switchToMineTrap()
    {
        if (debugLog)
        { Debug.Log("Switched to mine"); }
        builderObject.block = allBlockPrefabs[5];
        builderObject.type = "mine";
        builderObject.currentCost = blockPrice[5];

        switchSelectionColor(allButtons[5].GetComponent<Image>());
    }


    /******************* Pair 6 and Pair 7 ************************/
    public void switchToSmokeTrap()
    {
        if (debugLog)
        { Debug.Log("Switched to Smoke"); }
        builderObject.block = allBlockPrefabs[6];
        builderObject.type = "smoke";
        builderObject.currentCost = blockPrice[6];

        switchSelectionColor(allButtons[6].GetComponent<Image>());
    }


    
    public void switchToSpikeTrap()
    {
        if (debugLog)
        { Debug.Log("Switched to spike"); }
        builderObject.block = allBlockPrefabs[7];
        builderObject.type = "spikes";
        builderObject.currentCost = blockPrice[7];

        switchSelectionColor(allButtons[7].GetComponent<Image>());
    }

    /******************* Pair 8 and Pair 9 ************************/
    public void switchToGhostTrap()
    {
        if (debugLog)
        { Debug.Log("Switched to ghost"); }
        builderObject.block = allBlockPrefabs[8];
        builderObject.type = "ghost";
        builderObject.currentCost = blockPrice[8];

        switchSelectionColor(allButtons[8].GetComponent<Image>());
    }

    public void switchToTarTrap()
    {
        if (debugLog)
        { Debug.Log("Switched to tar"); }
        builderObject.block = allBlockPrefabs[9];
        builderObject.type = "tar";
        builderObject.currentCost = blockPrice[9];

        switchSelectionColor(allButtons[9].GetComponent<Image>());
    }

    //to color the button to signify what selection is selected
    private void switchSelectionColor(Image image) {

        prevSelected = currSelected;
        prevSelected.color = prevSelectedColor;
        //Debug.Log("Switched to color" + i);
        currSelected = image;
        currSelected.color = selectedColor;

    }

    public void turnOffButtons(string displaySet) {

        //enable offensive buttons, disable defenseive
        if (displaySet.Equals("set_1"))
        {
            for (int i = 0; i < allButtons.Length; i++) {

                if (i < allButtons.Length/2)
                {
                    allButtons[i].SetActive(true);
                }
                else {
                    allButtons[i].SetActive(false);
                }
                
            }
        }
        //enable defensive set of buttons, disable offensive
        else if (displaySet.Equals("set_2")) {
            for (int i = 0; i < allButtons.Length; i++)
            {

                if (i < allButtons.Length/2)
                {
                    allButtons[i].SetActive(false);
                }
                else
                {
                    allButtons[i].SetActive(true);
                }

            }
        }


    }

    //one of the arrows were pressed, switch to set1 or set2 based on last set used
    public void arrowPressed() {

        if (whichBlockSet.Equals("set_1"))
        {
            whichBlockSet = "set_2";
        }
        else if (whichBlockSet.Equals("set_2")) {
            whichBlockSet = "set_1";
        }

        turnOffButtons(whichBlockSet);

    }

}
