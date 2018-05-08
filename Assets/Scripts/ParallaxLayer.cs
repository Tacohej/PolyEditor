using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
	public float parallaxWeight;
	public float zPosition;

	void Update ()
	{
		var parallaxPosition = Camera.main.transform.position * parallaxWeight;
		this.transform.position = new Vector3(parallaxPosition.x, parallaxPosition.y, zPosition);
	}
}
