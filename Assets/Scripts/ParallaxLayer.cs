using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
	private Camera mainCamera;
	private float sortOrder;
	public float parallaxWeight = 0;
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

	void Update ()
	{
		// fix
		if (!mainCamera)
		{
			mainCamera = Camera.main;
		}

		if (mainCamera)
		{
			this.transform.position = (Vector2)mainCamera.transform.position * parallaxWeight;
		}
		this.transform.position = new Vector3(transform.position.x, transform.position.y, sortOrder);
	}
}
