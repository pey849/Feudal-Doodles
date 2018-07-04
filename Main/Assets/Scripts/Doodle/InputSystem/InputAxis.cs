namespace Doodle.InputSystem
{
    public enum InputAxis
    {
        /// <summary>
        /// The x-axis of the touch input.
        /// [ 0.0, +1.0 ]
        /// </summary>
        TouchX,

        /// <summary>
        /// The y-axis of the touch input.
        /// [ 0.0, +1.0 ]
        /// </summary>
        TouchY,

        /// <summary>
        /// The x-axis of the motion input. ( often "the left thumbstick" )
        /// [ -1.0, +1.0 ]
        /// </summary>
        MotionX,
         
        /// <summary>
        /// The y-axis of the motion input. ( often "the left thumbstick" )
        /// [ -1.0, +1.0 ]
        /// </summary>
        MotionY,

        /// <summary>
        /// The x-axis of the camera/view  input. ( often "the right thumbstick" )
        /// [ -1.0, +1.0 ]
        /// </summary>
        CameraX,

        /// <summary>
        /// The y-axis of the camera/view input. ( often "the right thumbstick" )
        /// [ -1.0, +1.0 ]
        /// </summary>
        CameraY
    }
}
