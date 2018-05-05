using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEditor{
	public class LevelData : ScriptableObject
	{
		public LayerData[] layers;
	}

	[System.Serializable]
	public struct LayerData
	{
		public string name;
		public float zPosition;
		public float parallaxWeight;
		public Vector2Int gridSize; // todo: replace with index in datablocks
		public TriangleBlockData[] triangleDataBlocks;
	}

	[System.Serializable]
	public struct TriangleBlockData
	{
		public Vector3 position;
		public bool[] triangles;
	}

}
	
