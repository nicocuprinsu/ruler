using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stretch : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var topRightCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
		var worldSpaceWidth = topRightCorner.x * 2;
		var worldSpaceHeight = topRightCorner.y * 2;

		var spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size;
		var scaleFactorX = worldSpaceWidth / spriteSize.x;
		var scaleFactorY = worldSpaceHeight / spriteSize.y;

		gameObject.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
