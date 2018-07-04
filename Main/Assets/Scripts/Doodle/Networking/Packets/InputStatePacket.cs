using Doodle.Game;
using Doodle.InputSystem;
using Doodle.Networking.Serialization;
using System;
using UnityEngine;

namespace Doodle.Networking.Packets
{
    [Packet( PacketType.Input )]
    public struct InputStatePacket
    {
        public PlayerId PlayerId;

        [SerializeField]
        private float[] _Axii;

        [SerializeField]
        private bool[] _Buttons;

        public InputStatePacket( PlayerId id, InputHandler handler )
        {
            PlayerId = id;

            var buttonEnums = (InputButton[]) Enum.GetValues( typeof( InputButton ) );
            var axisEnums = (InputAxis[]) Enum.GetValues( typeof( InputAxis ) );

            // Store Axis
            _Axii = new float[axisEnums.Length];
            foreach( var axis in axisEnums )
                _Axii[(int) axis] = handler.GetAxis( axis );

            // Store Button
            _Buttons = new bool[buttonEnums.Length];
            foreach( var button in buttonEnums )
            {
                var state = handler.GetButton( button );
                _Buttons[(int) button] = ( state == ButtonState.JustPressed )
                                     || ( state == ButtonState.Pressed );
            }
        }

        /// <summary>
        /// Returns true if the given button is 'held'.
        /// </summary>
        public bool GetButton( InputButton button )
        {
            return _Buttons[(int) button];
        }

        /// <summary>
        /// Returns the value of the given input axis. This is generally in the -1.0 to +1.0 range.
        /// </summary>
        public float GetAxis( InputAxis axis )
        {
            return _Axii[(int) axis];
        }
    }
}
