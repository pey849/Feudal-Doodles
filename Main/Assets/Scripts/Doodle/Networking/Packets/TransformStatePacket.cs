using Doodle.Game;
using Doodle.Networking.Serialization;
using UnityEngine;

namespace Doodle.Networking.Packets
{
    [Packet( PacketType.Transform )]
    public struct TransformStatePacket
    {
        public PlayerId PlayerId;

        public Vector2 Position;

        public float Angle;

        public TransformStatePacket( PlayerId id, Transform transform )
        {
            PlayerId = id; 
            Angle = transform.rotation.eulerAngles.z;
            Position = transform.position;
        }
    }
}