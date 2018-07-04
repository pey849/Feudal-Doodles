using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockDescriptionsHandler : MonoBehaviour {

    public Image[] blocks;
    public Text[] descriptions;
    public Image currImage;
    public Text currDescription;
    public Text currTitle;
    public GameObject overLayScreen;

    public void exitScreen() {
        overLayScreen.SetActive(false);
    }

    public void switchToNormalBloc()
    {
        currImage.GetComponent<Image>().sprite = blocks[0].sprite;
        currTitle.text = "Normal Block";
        currDescription.text = descriptions[0].text;
        overLayScreen.SetActive(true);
    }

    public void switchToCannonTrap()
    {
        currImage.GetComponent<Image>().sprite = blocks[1].sprite;
        currTitle.text = "Cannon Trap";
        currDescription.text = descriptions[1].text;
        overLayScreen.SetActive(true);
    }


    public void switchToTarTrap()
    {
        currImage.GetComponent<Image>().sprite = blocks[2].sprite;
        currTitle.text = "Tar Trap";
        currDescription.text = descriptions[2].text;
        overLayScreen.SetActive(true);
    }



    public void switchToIceBlock()
    {

        currImage.GetComponent<Image>().sprite = blocks[3].sprite;
        currTitle.text = "Ice Block";
        currDescription.text = descriptions[3].text;
        overLayScreen.SetActive(true);

    }


    public void switchToSmokeTrap()
    {
        currImage.GetComponent<Image>().sprite = blocks[4].sprite;
        currTitle.text = "Smoke Bomb Trap";
        currDescription.text = descriptions[4].text;
        overLayScreen.SetActive(true);
    }

    public void switchToMineTrap()
    {
        currImage.GetComponent<Image>().sprite = blocks[5].sprite;
        currTitle.text = "Mine Trap";
        currDescription.text = descriptions[5].text;
        overLayScreen.SetActive(true);
    }


    public void switchToOneTimeBlock()
    {
        currImage.GetComponent<Image>().sprite = blocks[6].sprite;
        currTitle.text = "One Time Block";
        currDescription.text = descriptions[6].text;
        overLayScreen.SetActive(true);
    }


    public void switchToSpikeTrap()
    {
        currImage.GetComponent<Image>().sprite = blocks[7].sprite;
        currTitle.text = "Spike Trap";
        currDescription.text = descriptions[7].text;
        overLayScreen.SetActive(true);
    }

    public void switchToGhostTrap()
    {
        currImage.GetComponent<Image>().sprite = blocks[8].sprite;
        currTitle.text = "One-Way Block";
        currDescription.text = descriptions[8].text;
        overLayScreen.SetActive(true);
    }

    public void switchToTrampolineBlock()
    {
        currImage.GetComponent<Image>().sprite = blocks[9].sprite;
        currTitle.text = "Trampoline Block";
        currDescription.text = descriptions[9].text;
        overLayScreen.SetActive(true);
    }


}
