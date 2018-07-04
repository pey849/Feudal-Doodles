using System;

namespace Doodle.InputSystem
{
    [Flags]
    public enum ButtonState
    {
        /// <summary>
        /// A button is not held.
        /// </summary>
        Released = 1 << 1,

        /// <summary>
        /// A button was just dicovered to be not held this frame.
        /// </summary>
        JustReleased = Released | ( 1 << 2 ),

        /// <summary>
        /// A button was just discovered to be pressed this frame.
        /// </summary>
        JustPressed = Pressed | ( 1 << 2 ),

        /// <summary>
        /// A button is held.
        /// </summary>
        Pressed = 1 << 0
    }
}
