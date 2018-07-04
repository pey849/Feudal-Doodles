using Doodle.Networking.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Doodle.Networking.Packets
{
    [Packet(PacketType.RunnerState)]
    [System.Serializable]
    public struct RunnerState
    {
        public Runner runnerPurple;
        public Runner runnerYellow;
        public bool fromHost;
        public bool fromPurple;

        public RunnerState(GameObject purple, GameObject yellow)
        {
            fromHost = false;
            fromPurple = false;
            Rigidbody2D purpleRB = purple.GetComponent<Rigidbody2D>();
            Rigidbody2D yellowRB = yellow.GetComponent<Rigidbody2D>();
            runnerPurple = new Runner();
            runnerYellow = new Runner();
            runnerPurple.position = new Vector3(purple.transform.position.x, purple.transform.position.y, 6f);
            runnerYellow.position = new Vector3(yellow.transform.position.x, yellow.transform.position.y, 6f);
            runnerPurple.AnglarVel = purpleRB.angularVelocity;
            runnerPurple.velocity = purpleRB.velocity;
            runnerPurple.rotation = purpleRB.rotation;
            runnerYellow.AnglarVel = yellowRB.angularVelocity;
            runnerYellow.velocity = yellowRB.velocity;
            runnerYellow.rotation = yellowRB.rotation;
            runnerPurple.scale = purple.transform.localScale;
            runnerYellow.scale = yellow.transform.localScale;
            runnerPurple.jump = purple.GetComponent<Animator>().GetBool("Jumping");
            runnerYellow.jump = yellow.GetComponent<Animator>().GetBool("Jumping");
            runnerPurple.speed = purple.GetComponent<Animator>().GetFloat("Speed");
            runnerYellow.speed = yellow.GetComponent<Animator>().GetFloat("Speed");
        }

        [System.Serializable]
        public class Runner
        {
            public Vector3 position;
            public float AnglarVel;
            public Vector2 velocity;
            public float rotation;
            public Vector3 scale;
            public bool jump;
            public float speed;
        }
    }
}