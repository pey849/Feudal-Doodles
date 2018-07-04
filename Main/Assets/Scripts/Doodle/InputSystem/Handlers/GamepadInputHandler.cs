using UnityEngine;

namespace Doodle.InputSystem.Handlers
{
    /// <summary>
    /// Maps gamepad/unity input to various buttons/axii. 
    /// This doesn't set the touch input.
    /// </summary>
    public class GamepadInputHandler : InputHandler
    {
        protected override void PollInput()
        {
            // Controller Axis
            SetAxis( InputAxis.MotionX, Input.GetAxisRaw( "Horizontal" ) );
            SetAxis( InputAxis.MotionY, Input.GetAxisRaw( "Vertical" ) );
            SetAxis( InputAxis.CameraX, 0 ); // TODO: Get right thumb-x
            SetAxis( InputAxis.CameraY, 0 ); // TODO: Get right thumb-y

            // Controller Buttons
            SetButton( InputButton.Jump, Input.GetButton( "Jump" ) );    // "A"
            SetButton( InputButton.Sprint, Input.GetButton( "Fire1" ) ); // "X"
            SetButton( InputButton.Grab, Input.GetButton( "Fire2" ) );   // "B"

            // Controller Ignores "Touch" inputs
        }
    }
}
