using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PolyEditor
{
	public class Controls : MonoBehaviour {

		// Public
		public float panSpeed;
		public float zoomSpeed;
		
		// Private
		private float maxZoom = 1f;
		private float minZoom = 20f;
		private Camera editorCamera;
		
		void Start()
		{
			editorCamera = GetComponent<Camera>();
		}

		void Update ()
		{
			var horizontalInput = Input.GetAxis("Horizontal");
			var verticalInput = Input.GetAxis("Vertical");
			var mouseScrollInput = Input.GetAxis("Mouse ScrollWheel");

			// Zoom faster when camera is further away
			var realZoomSpeed = mouseScrollInput * editorCamera.orthographicSize;
			var realPanSpeed = Time.deltaTime * panSpeed * editorCamera.orthographicSize;
			
			// Apply zoom to camera
			if (mouseScrollInput != 0)
			{
				editorCamera.orthographicSize = Mathf.Clamp(
					editorCamera.orthographicSize - realZoomSpeed, 
					maxZoom, 
					minZoom
				);
			}

			// Apply panning to camera postion
			this.transform.position += new Vector3(
				horizontalInput * realPanSpeed,
				verticalInput * realPanSpeed, 0
			);
		}
	}
}
