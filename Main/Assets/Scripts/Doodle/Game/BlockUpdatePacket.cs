using Doodle.Networking.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Doodle.Game;

namespace Doodle.Networking.Packets
{
    [Packet(PacketType.BlockUpdatePacket)]
    [System.Serializable]
    public struct BlockUpdatePacket
    {
        public Vector3 position;
        public string type;
        public TeamRole role;
        public bool activated;
        public bool isTrap;
        public bool blockWasAdded;

        public BlockUpdatePacket(Vector3 pos, string t, bool bwa)
        {
            position = pos;
            type = t;
            activated = false;
            isTrap = false;
            blockWasAdded = bwa;
            role = TeamRole.PurpleBuilder;
        }
    }
}