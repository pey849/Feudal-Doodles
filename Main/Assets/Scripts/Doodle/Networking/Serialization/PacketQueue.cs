using System;
using System.Collections.Generic;
using UnityEngine;

namespace Doodle.Networking.Serialization
{
    public class PacketQueue
    {
		// set to private when fixed in game engine.cs
		public Dictionary<Type, Queue<TypedPacket<object>>> Queue;

        /// <summary>
        /// The serializer used to destruct objects and reconstruct objects as byte representation.
        /// </summary>
        public PacketSerializer Serializer { get; private set; }

        public PacketQueue( PacketSerializer serializer )
        {
			Queue = new Dictionary<Type, Queue<TypedPacket<object>>>();
            Serializer = serializer;
        }

        /// <summary>
        /// Receive packest from the given network host.
        /// </summary> 
        public void GatherPackets( NetworkHost host )
        {
            if( host == null )
                throw new ArgumentNullException( "host" );

			Packet packet;
            while( host.Receive( out packet ) )
            {
                // Reconstruct object
                var obj = Serializer.Deserialize( packet.Data );

				TypedPacket<object> typedPacket = new TypedPacket<object>();
				typedPacket.Data = obj;
				typedPacket.Connection = packet.Connection;
				typedPacket.Channel = packet.Channel;

                //Debug.LogFormat( "Received a {0} ( {1} bytes )", obj.GetType().Name, packet.Data.Length );

                // If packet type is not yet encountered, created queue for that type.
                if( Queue.ContainsKey( obj.GetType() ) == false )
					Queue[obj.GetType()] = new Queue<TypedPacket<object>>();

                // Add packet object to queue.
				Queue[obj.GetType()].Enqueue( typedPacket );
            }
        }

        /// <summary>
        /// Gets the next available packet of the given type.
        /// </summary>
		public T GetNextPacket<T>()
        {
			return (T) GetNextPacket( typeof( T ) );
        }

		public T GetNextPacket<T>(out INetworkConnection nwConn)
		{
			return (T) GetNextPacket( typeof( T ), out nwConn );
		}

        /// <summary>
        /// Gets the next available packet of the given type.
        /// </summary>
		public object GetNextPacket( Type type )
        {
            if( HasPacket( type ) )
			{
				TypedPacket<object> typedPacket = Queue[type].Dequeue();
				return typedPacket.Data;
			}

            // No packet
            return null;
        }

		public object GetNextPacket( Type type, out INetworkConnection nwConn)
		{
			if( HasPacket( type ) )
			{
				TypedPacket<object> typedPacket = Queue[type].Dequeue();
				nwConn = typedPacket.Connection;
				return typedPacket.Data;
			}

			// No packet
			nwConn = null;
			return null;
		}
			
        /// <summary>
        /// Determines if a packet exists if the given type.
        /// </summary>
        public bool HasPacket<T>()
        {
            return HasPacket( typeof( T ) );
        }

        /// <summary>
        /// Determines if a packet exists if the given type.
        /// </summary>
        public bool HasPacket( Type type )
        {
            if( Queue.ContainsKey( type ) )
            {
                var queue = Queue[type];
                return queue.Count > 0;
            }

            // Nope
            return false;
        }
    }
}
