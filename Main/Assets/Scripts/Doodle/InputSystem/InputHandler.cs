using System;
using System.Collections.Generic;
using UnityEngine;

namespace Doodle.InputSystem
{
    /// <summary>
    /// Input handler, contains a mapping of input values.
    /// </summary>
    public abstract class InputHandler : MonoBehaviour
    {
        private Dictionary<InputAxis, float> AxisMap;
        private Dictionary<InputButton, bool> ButtonBoolMap;
        private Dictionary<InputButton, ButtonState> ButtonStateMap;

        protected InputHandler()
        {
            // 
            AxisMap = new Dictionary<InputAxis, float>();
            foreach( InputAxis axis in Enum.GetValues( typeof( InputAxis ) ) )
                AxisMap[axis] = default( float );

            // 
            ButtonBoolMap = new Dictionary<InputButton, bool>();
            foreach( InputButton button in Enum.GetValues( typeof( InputButton ) ) )
                ButtonBoolMap[button] = default( bool );

            // 
            ButtonStateMap = new Dictionary<InputButton, ButtonState>();
            foreach( InputButton button in Enum.GetValues( typeof( InputButton ) ) )
                ButtonStateMap[button] = default( ButtonState );
        }

        protected abstract void PollInput();

        void Update()
        {
            // 
            PollInput();

            // Evaluate changes in the button mapping to determine the "just" states
            foreach( InputButton button in Enum.GetValues( typeof( InputButton ) ) )
            {
                var state = ButtonStateMap[button];

                if( ButtonBoolMap[button] )
                {
                    if( state == ButtonState.Released ) state = ButtonState.JustPressed;
                    else state = ButtonState.Pressed;
                }
                else
                {
                    if( state == ButtonState.Pressed ) state = ButtonState.JustReleased;
                    else state = ButtonState.Released;
                }

                ButtonStateMap[button] = state;
            }
        }

        /// <summary>
        /// Sets the value of the given axis.
        /// </summary>
        protected void SetAxis( InputAxis axis, float value )
        {
            AxisMap[axis] = value;
        }

        /// <summary>
        /// Set the "held or not" value of the given button.
        /// </summary>
        protected void SetButton( InputButton button, bool pressed )
        {
            ButtonBoolMap[button] = pressed;
        }

        /// <summary>
        /// Gets the value of the given axis.
        /// </summary>
        public float GetAxis( InputAxis axis )
        {
            if( AxisMap.ContainsKey( axis ) ) return AxisMap[axis];
            else throw new InvalidOperationException( string.Format( "Unable to find button name '{0}'.", axis ) );
        }

        /// <summary>
        /// Gets the state of the given button.
        /// </summary>
        public ButtonState GetButton( InputButton button )
        {
            if( ButtonStateMap.ContainsKey( button ) ) return ButtonStateMap[button];
            else throw new InvalidOperationException( string.Format( "Unable to find button name '{0}'.", button ) );
        }
    }
}
