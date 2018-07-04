using Doodle.Networking.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.Game;
using System;

namespace Doodle.Networking.Packets
{
    [Packet(PacketType.pushPacket)]
    [System.Serializable]
    public struct pushPacket
    {
        public bool fromHost;
        public bool fromPurple;
        public Vector2 dir;

        public pushPacket(bool h)
        {
            fromHost = false;
            fromPurple = true;
            dir = Vector2.left;
        }
    }
}