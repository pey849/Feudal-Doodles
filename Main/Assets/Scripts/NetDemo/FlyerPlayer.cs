using Doodle.InputSystem;
using UnityEngine;

public class FlyerPlayer : MonoBehaviour
{
    public InputHandler Input;

    private Rigidbody2D RB;

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if( Input != null )
        {
            Vector2 force = Vector2.zero;
            force.x = Input.GetAxis( InputAxis.MotionX );
            force.y = Input.GetAxis( InputAxis.MotionY );
            RB.AddForce( force, ForceMode2D.Impulse );
        }
    }
}
