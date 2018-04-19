using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanning : MonoBehaviour {
	public float speed = 10f;

	void Update ()
	{
		var horizontalInput = Input.GetAxis("Horizontal");
		var verticalInput = Input.GetAxis("Vertical");
		var actualSpeed = Time.deltaTime * speed;

		this.transform.position += new Vector3(
			horizontalInput * actualSpeed,
			verticalInput * actualSpeed,
			0
		);
	}
}
