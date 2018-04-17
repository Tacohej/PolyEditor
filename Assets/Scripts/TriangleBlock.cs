using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriangleLocation { UP, LEFT, DOWN, RIGHT }
public class TriangleBlock : MonoBehaviour
{
	[System.Serializable]
	public struct Struct
	{
		public bool[] triangles;
		public Vector3 position;
		public Struct (Vector3 _position, GameObject[] _triangles)
		{
			position = _position;
			triangles = new bool[4] {
				_triangles[0],
				_triangles[1],
				_triangles[2],
				_triangles[3]
			};
		}
	}

	public GameObject[] triangles;

	void Start ()
	{
		var meshFilter = this.gameObject.AddComponent<MeshFilter>();
		this.gameObject.AddComponent<MeshRenderer>();
		meshFilter.mesh = MeshUtility.GenerateMarker();
		triangles = new GameObject[4];
	}

	public Struct GenerateStruct ()
	{
		return new Struct(this.transform.position, triangles);
	}

	public bool hasTriangles () {
		return triangles[0] != null ||
			triangles[1] != null ||
			triangles[2] != null || 
			triangles[3] != null;
	}
}

