using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PaperFx : MonoBehaviour
{
    public float PaperScale = 1;
    public float PaperDistortion = 0.001F;
    public Texture2D PaperTexture;

    public float PaperMixStrength = 1F;

    private Material _Material;
    private Camera _Camera;
	private bool AndroidEnvironment = false;

    // Creates a private material used to the effect
    void Awake()
    {
        _Material = new Material( Shader.Find( "Hidden/PaperFx" ) );
        _Camera = GetComponent<Camera>();

		#if UNITY_ANDROID
		AndroidEnvironment = true;
		#endif

    }

    // Postprocess the image
    void OnRenderImage( RenderTexture source, RenderTexture destination )
    {
        if( PaperTexture == null ) Graphics.Blit( source, destination );
        else
        {
            var v1 = _Camera.ViewportToWorldPoint( Vector2.zero );
            var v2 = _Camera.ViewportToWorldPoint( Vector2.one );
            //-1F for Android devices and 1F for PC
			float scale_y = 1F / (v2.y - v1.y);;
			if (AndroidEnvironment) {
				scale_y = -1F / (v2.y - v1.y);
			}
            var scale_x = 1F / ( v2.x - v1.x );


            // 
            var offset = (Vector2) _Camera.transform.position;
            offset.x *= scale_x;
            offset.y *= -scale_y;

            var textureAspect = PaperTexture.width / (float) PaperTexture.height;

            _Material.SetVector( "_PaperOffset", offset );
            _Material.SetVector( "_PaperScale", new Vector2( _Camera.aspect / textureAspect, 1 ) * PaperScale );

            // 
            _Material.SetTexture( "_PaperTex", PaperTexture );

            //
            _Material.SetFloat( "_PaperBlend", PaperMixStrength );
            _Material.SetFloat( "_PaperDistort", PaperDistortion );

            Graphics.Blit( source, destination, _Material );
        }
    }
}