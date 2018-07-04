using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System;
using System.Xml.Serialization;
using System.Xml;
using Doodle.Networking.Packets;

public class GameEngineFuedal : MonoBehaviour
{
    private GameState currentState;
    //private volatile bool updateState = false;
    //private volatile bool updateRender = false;

    //// Use this for initialization
    //void Start()
    //{
    //    //start a thread to wait for clients
    //    //temp for testing
    //    new Thread(delegate ()
    //    {
    //        try
    //        {
    //            //open prep and start tcp socket server
    //            TcpListener serverSocket = new TcpListener(1119);
    //            TcpClient clientSocket = default(TcpClient);
    //            serverSocket.Start();
    //            clientSocket = serverSocket.AcceptTcpClient();

    //            //wait for a network stream from client for initial connection
    //            byte[] bytesFrom = new byte[10025];
    //            string dataFromClient = null;
    //            NetworkStream networkStream = clientSocket.GetStream();
    //            networkStream.Read(bytesFrom, 0, bytesFrom.Length);
    //            dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);

    //            //once new client connects, start new thread
    //            new Thread(delegate ()
    //            {
    //                //loop waiting for input from clients
    //                while ((true))
    //                {
    //                    bytesFrom = new byte[10025];
    //                    dataFromClient = null;

    //                    try
    //                    {
    //                        //listen for input from client
    //                        networkStream.Read(bytesFrom, 0, bytesFrom.Length);
    //                        dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
    //                        Regex rgx = new Regex("[^$a-zA-Z0-9?!.,'\" \"]");
    //                        dataFromClient = rgx.Replace(dataFromClient, "");

    //                        //print what client says
    //                        Debug.Log(dataFromClient);

    //                        string[] touchLocation = dataFromClient.Split(' ', '\t');
    //                        float xTemp = float.Parse(touchLocation[0]);
    //                        float yTemp = float.Parse(touchLocation[1]);

    //                        //GameState state = generateState();
    //                        updateState = true;

    //                        //add new block to state
    //                        GameState.Block b2 = new GameState.Block();
    //                        b2.type = "normal";
    //                        b2.x = xTemp;
    //                        b2.y = yTemp;
    //                        //currentState.blocks.Add(b2);
    //                        currentState.blocks[currentState.nextOpen] = b2;
    //                        currentState.nextOpen++;

    //                        updateRender = true;
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        Debug.Log("there was an error with client connection :(");
    //                        return;
    //                    }
    //                }
    //            }).Start();
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.Log("There wan an error waiting for client connection :(");
    //            return;
    //        }
    //    }).Start();
    //}


    //// Update is called once per frame
    ////calling render functions in update using voltile bools in order to easily call function on main thread
    ////(the render function can only be called from unities main thread)
    //void Update()
    //{
    //    if (updateState)
    //    {
    //        currentState = generateState();
    //        updateState = false;
    //    }

    //    if (updateRender)
    //    {
    //        RenderGameState(currentState);
    //        updateRender = false;
    //    }
    //}

    ////function to generate current gamestates
    //GameState generateState()
    //{
    //    GameState cs = new GameState();
    //    GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
    //    foreach (GameObject block in blocks)
    //    {
    //        GameState.Block b1 = new GameState.Block();
    //        b1.type = "normal";
    //        b1.x = block.transform.position.x;
    //        b1.y = block.transform.position.y;
    //        //cs.blocks.Add(b1);
    //        cs.blocks[cs.nextOpen] = b1;
    //        cs.nextOpen++;
    //    }
    //    return cs;
    //}

    //void RenderGameState(GameState state)
    //{
    //    //render gamestate
    //    GameObject.Find("Renderer").gameObject.GetComponent<Renderer>().renderAllBlocksAsNew(state);
    //}
}