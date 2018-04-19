using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
	private Camera mainCamera;

	private float sortOrder;
	private float parallaxWeight = 0;
	public void SetCamera (Camera newCamera)
	{
		mainCamera = newCamera;
	}
	public void SetSortOrder (float newSortOrder)
	{
		sortOrder = newSortOrder;
	}
	public void SetParallaxWeight(float newParallaxWeight)
	{
		parallaxWeight = newParallaxWeight;
	}
	void LateUpdate ()
	{
		if (mainCamera)
		{
			this.transform.position = (Vector2)mainCamera.transform.position * parallaxWeight;
		}
		this.transform.position = new Vector3(transform.position.x, transform.position.y, sortOrder);
	}
}
