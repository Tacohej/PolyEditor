using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEditor
{
	public enum TriangleLocation { UP, LEFT, DOWN, RIGHT }

	[System.Serializable]
	public struct TriangleBlockData
	{
		public Vector3 position;
		public bool[] triangles;
	}
	public class TriangleBlock : MonoBehaviour
	{
		private GameObject[] triangles = new GameObject[4];

		public void AddTriangle(TriangleLocation location, Mesh[] triangleMeshes)
		{
			var index = (int)location;

			if (!triangles[index])
			{
				var triangle = GenerateTriangleGameObject(this.transform.position);
				triangle.GetComponent<MeshFilter>().mesh = triangleMeshes[index];
				triangle.name = location.ToString();
				
				triangle.transform.parent = this.transform;
				triangles[index] = triangle;
			}
		}

		public void RemoveTriangle (int index)
		{
			if(triangles[index]){
				triangles[index].transform.parent = null;
				Destroy(triangles[index]);
				triangles[index] = null;
			}
		}

		GameObject GenerateTriangleGameObject (Vector3 position)
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<MeshFilter>();
			gameObject.AddComponent<MeshRenderer>();
			gameObject.transform.position = position;
			return gameObject;
		}

		public TriangleBlockData ToData ()
		{
			var data = new TriangleBlockData();
			data.triangles = new bool[4];
			data.position = this.transform.position;
			data.triangles[0] = triangles[0];
			data.triangles[1] = triangles[1];
			data.triangles[2] = triangles[2];
			data.triangles[3] = triangles[3];
			return data;
		}
	}
}

