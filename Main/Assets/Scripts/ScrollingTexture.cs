using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTexture : MonoBehaviour {

	//the texture renderer of the background
    private Material textureMaterial;

	//the horizontal scroll speed of the background
    public float xScrollSpeed;

    //the vertical scroll speed of the background
    public float yScrollSpeed;

	// Use this for initialization
	void Start () {
		//set the material
        this.textureMaterial = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		//scroll the texture by the given input speed
		this.textureMaterial.mainTextureOffset = new Vector2(this.textureMaterial.mainTextureOffset.x + this.xScrollSpeed,
                    this.textureMaterial.mainTextureOffset.y + this.yScrollSpeed);
		
		//prevent overflow by reseting the texture offsets
		if(this.textureMaterial.mainTextureOffset.x > 1)
		{
			this.textureMaterial.mainTextureOffset = new Vector2(this.textureMaterial.mainTextureOffset.x - 1,
                    this.textureMaterial.mainTextureOffset.y);
		}
		else if(this.textureMaterial.mainTextureOffset.x < -1)
		{
			this.textureMaterial.mainTextureOffset = new Vector2(this.textureMaterial.mainTextureOffset.x + 1,
                    this.textureMaterial.mainTextureOffset.y);
		}
		//do the same for the y offset
		if(this.textureMaterial.mainTextureOffset.y > 1)
		{
			this.textureMaterial.mainTextureOffset = new Vector2(this.textureMaterial.mainTextureOffset.x,
                    this.textureMaterial.mainTextureOffset.y - 1);
		}
		else if(this.textureMaterial.mainTextureOffset.y < -1)
		{
			this.textureMaterial.mainTextureOffset = new Vector2(this.textureMaterial.mainTextureOffset.x,
                    this.textureMaterial.mainTextureOffset.y + 1);
		}

	}
}
