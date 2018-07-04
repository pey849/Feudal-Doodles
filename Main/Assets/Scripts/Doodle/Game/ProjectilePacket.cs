using Doodle.Networking.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.Game;
using System;

namespace Doodle.Networking.Packets
{
    [Packet(PacketType.ProjectilePacket)]
    [System.Serializable]
    public struct ProjectilePacket
    {
        public bool instanti;
        public string type;
        public Vector3 pos;
        public DateTime timestamp;
        public float AnglarVel;
        public Vector2 velocity;
        public float rotation;
        public int ID;
        public bool formHost;
        public TeamRole role;

        public ProjectilePacket(bool instant, GameObject bullet)
        {
            ID = 0;
            formHost = false;
            timestamp = DateTime.Now;
            instanti = instant;
            type = "cannonball";
            pos = bullet.transform.position;
            AnglarVel = bullet.GetComponent<Rigidbody2D>().angularVelocity;
            velocity = bullet.GetComponent<Rigidbody2D>().velocity;
            rotation = bullet.GetComponent<Rigidbody2D>().rotation;
            role = TeamRole.PurpleRunner;
        }
    }
}