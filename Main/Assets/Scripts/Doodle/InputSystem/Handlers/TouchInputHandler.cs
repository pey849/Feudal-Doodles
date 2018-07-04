using System.Linq;
using UnityEngine;

namespace Doodle.InputSystem.Handlers
{
    /// <summary>
    /// A handler that sets the <see cref="InputButton.Touch"/>, <see cref="InputAxis.TouchX"/> and <see cref="InputAxis.TouchY"/> values.
    /// All other values are ignored.
    /// </summary>
    public class TouchInputHandler : InputHandler
    {
        private int FingerIndex = -1;

        private Touch GetTouchByIndex( int finger )
        {
            return Input.touches.First( p => p.fingerId == finger );
        }

        private bool IsFingerDown( int finger )
        {
            if( finger < 0 ) return false;
            return Input.touches.Where( p => p.fingerId == finger ).Any();
        }

        protected override void PollInput()
        {
            // If there is any touch
            if( Input.touchCount > 0 )
            {
                // Get new first finger
                if( FingerIndex < 0 )
                {
                    var touch = Input.GetTouch( 0 );
                    FingerIndex = touch.fingerId;
                }

                // 
                if( IsFingerDown( FingerIndex ) )
                {
                    var touch = GetTouchByIndex( FingerIndex );

                    // 
                    SetAxis( InputAxis.TouchX, touch.position.x / Screen.width );
                    SetAxis( InputAxis.TouchY, touch.position.y / Screen.height );

                    // 
                    SetButton( InputButton.Touch, true );
                }
            }
            else
            {
                // Reset to no touch state.
                SetAxis( InputAxis.TouchX, 0 );
                SetAxis( InputAxis.TouchY, 0 );
                SetButton( InputButton.Touch, false );
                FingerIndex = -1;
            }
        }
    }
}
