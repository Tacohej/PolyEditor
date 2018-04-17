using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLayer : ScriptableObject {

	//[System.Serializable]
	public Material material;
	public string layerName;
	public float sortOrder;
	public float parallaxWeight;
	public TriangleBlock.Struct[] triangleStructs;
}
