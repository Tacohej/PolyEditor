using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLayer : ScriptableObject {
	public string layerName;

	[Space(10)]
	public Material material;
	public bool collidable;

	[Tooltip("Determine the z value of the layer")]
	public float sortOrder;

	[Range(0f, 1.0f)]
	[Tooltip("Further away -> higher value")]
	public float parallaxWeight;
	public TriangleBlock.Struct[] dataStructs;
}
