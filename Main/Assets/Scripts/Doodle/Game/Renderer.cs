using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.Networking.Packets;
using System;
using Doodle.Game;

public class Renderer : MonoBehaviour
{
    public GameObject pencilEffect;
    public GameObject NormalBlock;
    public GameObject iceBlock;
    public GameObject fireBlock;
//<<<<<<< HEAD
    public GameObject cannonBlock;
    public GameObject smokeBlock;
    public GameObject mineBlock;
    public GameObject tempBlock;

    public GameObject tarBlock;
    public GameObject jumpPadBlock;
    public GameObject ghostBlock;
    public GameObject oneTimeBlock;
    public GameObject spikeBlock;

//=======
//    public GameObject cannonTrap;
//    public GameObject smokebomb;
//>>>>>>> develop

    public GameState cs;

    public GameEngine engine;

    // Use this for initialization
    void Start()
    {
        engine = GameObject.Find("Game Manager").GetComponent<GameEngine>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //reads in the objects in scene and generates the gamestate O(n)
    public GameState generateCurrentBlockState()
    {
        //only generates block states, does not do player/traps (yet)
        GameState state = new GameState(false);

        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in blocks)
        {
            try
            {
                GameState.Block b = new GameState.Block();
                b.type = block.GetComponent<Block>().type;
                b.position = block.transform.position;
                b.role = block.GetComponent<Block>().role;
                state.blocks.Add(b);
            } catch (Exception e)
            {
                print("ffs");
            }
        }

        state.timestamp = DateTime.Now;
        return state;
    }

    //figure out the changes between states, and call the single functions to modify it O(n^2)
    public void compareStatesBlocks(GameState newState)
    {
        List<GameState.Block> blocksToRemove = new List<GameState.Block>();

        GameState currentState = generateCurrentBlockState();

        //loop checks each block in the currentstate against blocks in newstate
        int countOuter = 0;
        foreach (GameState.Block currBlock in currentState.blocks)
        {
            //bool for determining if block in the current state, is not in the new state
            bool blockNotFound = true;

            int countInner = 0;
            foreach (GameState.Block newBlock in newState.blocks)
            {
                if (currBlock.position == newBlock.position && currBlock != null && newBlock != null && newBlock.type.Equals(currBlock.type))
                {
                    //new state block is found in the current state
                    blockNotFound = false;
                }
                countInner++;
            }

            if (blockNotFound)
            {
                RenderRemoveBlock(currBlock);
            }
            countOuter++;
        }

        //finally add blocks remaining in newState to currentState, as they must be new
        foreach (GameState.Block newBlock in newState.blocks)
        {
            bool blockNotFound = true;

            foreach (GameState.Block currBlock in currentState.blocks)
            {
                if (currBlock.position == newBlock.position && currBlock != null && newBlock != null && newBlock.type.Equals(currBlock.type))
                {
                    blockNotFound = false;
                }
            }
            if (blockNotFound)
            {
                DateTime t1 = DateTime.Now;
                double delay = (newState.timestamp - t1).TotalSeconds;
                RenderAddBlock(newBlock, (float)delay);
            }
        }
    }

    //defaults delay to 0
    public void RenderAddBlock(GameState.Block block)
    {
        RenderAddBlock(block, 0);
    }

    //when just a new block is sent, just render the single block O(1)
    public void RenderAddBlock(GameState.Block block, float delay)
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        GameObject added;
        GameObject addedHand;

        //go through all blocks to see if one is already at that locattion
        foreach (GameObject b in blocks)
        {
            print("pos1: "+b.transform.position);
            print("pos2: " + block.position);
            if ((Math.Abs(b.transform.position.x - block.position.x) < 0.5) && (Math.Abs(b.transform.position.y - block.position.y) < 0.5))
            {
                //don't place block if there is already one there
                return;
            }
        }

        //adds block to screen, and snaps it to nearest 0.5 (snapping to be used for "grid size" later)
        if (block.type.Equals("normal")/* && block.x > -90*/)
        {
            added = Instantiate(NormalBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        }
        else if (block.type.Equals("fire")/* && block.x > -90*/)
        {
            added = Instantiate(fireBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                 Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        }
        else if (block.type.Equals("ice") /*&& block.x > -90*/)
        {
            added = Instantiate(iceBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                 Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        }
//<<<<<<< HEAD
        else if (block.type.Equals("mine")/* && block.x > -90*/)
        {
            added = Instantiate(mineBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        }
        else if (block.type.Equals("smoke")/* && block.x > -90*/)
        {
            added = Instantiate(smokeBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                 Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        }
        else if (block.type.Equals("cannon") /*&& block.x > -90*/)
        {
            added = Instantiate(cannonBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                 Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        }
        else if (block.type.Equals("temp") /*&& block.x > -90*/)
        {
            added = Instantiate(tempBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                 Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        }
        else if(block.type.Equals("tar"))
        {
            added = Instantiate(tarBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                 Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        }
        else if (block.type.Equals("jumppad"))
        {
            added = Instantiate(jumpPadBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                 Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        }
        else if (block.type.Equals("ghost"))
        {
            added = Instantiate(ghostBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                 Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        }
        else if (block.type.Equals("onetime"))
        {
            added = Instantiate(oneTimeBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                 Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        }
        else if (block.type.Equals("spikes"))
        {
            added = Instantiate(spikeBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                 Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        }

        //        else {
        //=======
        //else if (block.type.Equals("cannon") /*&& block.x > -90*/)
        //{
        //    added = Instantiate(cannonTrap, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
        //         Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        //}
        //else if (block.type.Equals("smokebomb") /*&& block.x > -90*/)
        //{
        //    added = Instantiate(smokebomb, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
        //         Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        //}
        else {
//>>>>>>> develop
            added = Instantiate(NormalBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
        }
        added.GetComponent<Block>().delayFromServer = delay;
        added.GetComponent<Block>().role = block.role;

        //tint blocks based on team.
        SpriteRenderer[] pencilColor = added.GetComponentsInChildren<SpriteRenderer>();
        if (added.GetComponent<Block>().role == Doodle.Game.TeamRole.PurpleBuilder || added.GetComponent<Block>().role == Doodle.Game.TeamRole.PurpleRunner)
        {
            added.transform.GetComponent<SpriteRenderer>().color = new Color32(0xff, 0x90, 0xdc, 0xFF); //RGBA
            if (!block.type.Equals("smoke"))
            {
                pencilColor[1].color = new Color32(0xff, 0x90, 0xdc, 0xFF); //RGBA
            }
            else {

                pencilColor[0].color = new Color32(0xff, 0x90, 0xdc, 0xFF); //RGBA
            }
            

        } else
        {
            added.transform.GetComponent<SpriteRenderer>().color = new Color32(0xff, 0xff, 0x00, 0xFF); //RGBA
            if (!block.type.Equals("smoke"))
            {
                pencilColor[1].color = new Color32(0xff, 0xff, 0x00, 0xFF); //RGBA
            }
            else
            {
                pencilColor[0].color = new Color32(0xff, 0xff, 0x00, 0xFF); //RGBA
            }
        }
    }

    //when a block is removed, just remove the single block O(n)
    public void RenderRemoveBlock(GameState.Block block)
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");

        //go through all blocks until we find the one we need to remove
        foreach (GameObject b in blocks)
        {
            if (b.transform.position == block.position)
            {
                if (b.gameObject.GetComponent<Block>().type.Equals("onetime"))
                {
                    Destroy(b.gameObject);
                    return;
                }
                if (b.gameObject != null)
                {
                    DestroyImmediate(b.gameObject);
                }
                return;
            }
        }
    }

    //used to cmpletely rerender the gamestate, does not update. O(2n)<-not including instantiate
    public void renderAllBlocksAsNew(GameState currentState)
    {
        //get all block game objects, to delete after adding new ones
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");

        foreach (GameState.Block block in currentState.blocks)
        {
            //adds block to screen, and snaps it to nearest 0.5 (snapping to be used for "grid size" later)
            if (block.type.Equals("normal")/* && block.x > -90*/)
            {
                Instantiate(NormalBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                    Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
            }
            if (block.type.Equals("fire")/* && block.x > -90*/)
            {
                Instantiate(fireBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                    Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
            }
            if (block.type.Equals("ice") /*&& block.x > -90*/)
            {
                Instantiate(iceBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                    Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
            }

            if (block.type.Equals("mine")/* && block.x > -90*/)
            {
                Instantiate(mineBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                    Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
            }
            if (block.type.Equals("smoke")/* && block.x > -90*/)
            {
                Instantiate(smokeBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                     Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
            }
            if (block.type.Equals("cannon") /*&& block.x > -90*/)
            {
                Instantiate(cannonBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                     Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
            }

            if (block.type.Equals("temp") /*&& block.x > -90*/)
            {
                Instantiate(tempBlock, new Vector3(Mathf.Sign(block.position.x) * (Mathf.Abs((int)block.position.x) + 0.5f),
                     Mathf.Sign(block.position.y) * (Mathf.Abs((int)block.position.y) + 0.5f), 6f), Quaternion.identity);
            }
        }

        //loop through and remove blocks from last 'render'
        foreach (GameObject block in blocks)
        {
            //block.transform.position = new Vector2(0, 0);
            Destroy(block.gameObject);
        }
    }

	/// <summary>
	/// Remove all the blocks. This will be called when Game Engine needs to stop.
	/// </summary>
	public void RenderRemoveBlocksAll()
	{
		GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
		foreach (GameObject b in blocks)
			Destroy(b.gameObject);
	}
}