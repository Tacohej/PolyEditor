using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {
	public float panSpeed = 10f;
	public float zoomSpeed = 5f;
	
	private Camera editorCamera;
	private float maxZoom = 1;
	private float minZoom = 20;

	void Start()
	{
		editorCamera = GetComponent<Camera>();
	}
	void Update ()
	{
		var horizontalInput = Input.GetAxis("Horizontal");
		var verticalInput = Input.GetAxis("Vertical");
		var mouseScrollInput = Input.GetAxis("Mouse ScrollWheel");
		
		var realPanSpeed = Time.deltaTime * panSpeed;

		// Zoom faster when camera is further away
		var realZoomSpeed = mouseScrollInput * editorCamera.orthographicSize;

		// Apply zoom to camera
		if (mouseScrollInput != 0)
		{
			editorCamera.orthographicSize = Mathf.Clamp(editorCamera.orthographicSize - realZoomSpeed, maxZoom, minZoom);
		}

		// Apply panning to camera postion
		this.transform.position += new Vector3(horizontalInput * realPanSpeed, verticalInput * realPanSpeed, 0);
	}
}
