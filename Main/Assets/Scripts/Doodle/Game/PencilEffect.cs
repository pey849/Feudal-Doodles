using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PencilEffect : MonoBehaviour
{
    public Vector3 StartCoordinate;

    public Vector3 EndCoordinate;

    [Range( 0.25F, 5F )]
    public float DurationSeconds = 2F;

    [Range( 2, 20 )]
    public float WobbleFrequency = 7F;

    private float CurrentAnimationTime;

    // Use this for initialization
    void Start()
    {
        CurrentAnimationTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Animation time
        CurrentAnimationTime += Time.deltaTime;

        // Expire graphic
        if( CurrentAnimationTime > DurationSeconds )
        {
            // Kill itself
            Destroy( gameObject );
        }
        else
        {
            float progress = CurrentAnimationTime / DurationSeconds;

            // Fade animation
            float fade = Mathf.Pow( Mathf.Sin( progress * Mathf.PI ), 0.2F );
            float wobble = Mathf.Pow( Mathf.Sin( progress * WobbleFrequency * Mathf.PI ), 3F );

            // Fade Sprite
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = Fade( sr.color, fade );

            // Animate postion
            Vector3 wob = Perp( EndCoordinate - StartCoordinate ) * wobble;
            Vector3 pos = Vector3.Lerp( StartCoordinate, EndCoordinate, progress );
            transform.position = pos + wob;
        }
    }

    private Vector3 Perp( Vector3 v )
    {
        v.Normalize();
        return new Vector3( v.y, -v.x, v.z );
    }

    private Color Fade( Color color, float alpha )
    {
        color.a = alpha;
        return color;
    }
}
