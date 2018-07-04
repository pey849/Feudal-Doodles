using Doodle.Networking.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Doodle.Game;

namespace Doodle.Networking.Packets
{
    [Packet(PacketType.GameState)]
    [System.Serializable]
    public struct GameState
    {
        public DateTime timestamp;

        public bool fromHost;
        public List<Block> blocks;

        //for logs
        public List<Block> blocksAdded;
        public List<Block> blocksRemoved;

        public GameState(bool hi)
        {
            timestamp = DateTime.Now;
            fromHost = false;
            blocks = new List<Block>();
            blocksAdded = new List<Block>();
            blocksRemoved = new List<Block>();
        }
        
        [System.Serializable]
        public class Block
        {
            public Vector3 position;
            //change to enum
            public string type;
            //only used for trap type bloacks
            public bool activated;

            public TeamRole role;
        }
    }
}