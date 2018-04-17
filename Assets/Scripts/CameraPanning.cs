using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanning : MonoBehaviour {

	public float speed = 10f;

	void Update ()
	{
		var horizontalInput = Input.GetAxis("Horizontal");
		var verticalInput = Input.GetAxis("Vertical");

		this.transform.position += new Vector3(
			horizontalInput * Time.deltaTime * speed,
			verticalInput * Time.deltaTime * speed,
			0
		);
	}
}
