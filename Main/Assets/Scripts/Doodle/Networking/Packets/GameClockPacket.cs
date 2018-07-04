using Doodle.Networking.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Doodle.Networking.Packets
{
    [Packet(PacketType.GameClockPacket)]
    [System.Serializable]
    public struct GameClockPacket
    {
        public float currentTime;
        public int leftGold;
        public int rightGold;

        public GameClockPacket(bool hi)
        {
            currentTime = 0;
            leftGold = 0;
            rightGold = 0;
        }
    }
}
