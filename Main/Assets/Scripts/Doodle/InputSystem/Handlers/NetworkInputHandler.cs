using Doodle.Networking.Packets;
using System;
using System.Collections.Generic;

namespace Doodle.InputSystem.Handlers
{
    /// <summary>
    /// Receives input packets and process each packet one frame at a time, setting a complete input state.
    /// </summary>
    public class NetworkInputHandler : InputHandler
    {
        private Queue<InputStatePacket> States;

        // 
        private static InputButton[] ButtonEnums;
        private static InputAxis[] AxisEnums;

        static NetworkInputHandler()
        {
            ButtonEnums = (InputButton[]) Enum.GetValues( typeof( InputButton ) );
            AxisEnums = (InputAxis[]) Enum.GetValues( typeof( InputAxis ) );
        }

        void Start()
        {
            States = new Queue<InputStatePacket>();
        }

        /// <summary>
        /// Adds a state packet to the queue to be processed at the next frame.
        /// </summary>
        public void EnqueueStatePacket( InputStatePacket statePacket )
        {
            States.Enqueue( statePacket );
        }

        protected override void PollInput()
        {
            if( States.Count > 0 )
            {
                var state = States.Dequeue();

                // Update axis
                foreach( var axis in AxisEnums )
                    SetAxis( axis, state.GetAxis( axis ) );

                // Update buttons
                foreach( var button in ButtonEnums )
                    SetButton( button, state.GetButton( button ) );
            }
        }
    }
}