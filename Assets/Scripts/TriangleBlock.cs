using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEditor
{
	public enum TriangleLocation { UP, LEFT, DOWN, RIGHT }
	public class TriangleBlock : MonoBehaviour
	{
		private GameObject[] triangles = new GameObject[4];

		public void AddTriangle(TriangleLocation location)
		{
			var index = (int)location;

			if (!triangles[index])
			{
				CreateTriangle(index);
			}
		}

		public TriangleBlockData ToData ()
		{
			var data = new TriangleBlockData();
			data.triangles = new bool[4];
			data.position = this.transform.position;
			data.triangles[0] = !!triangles[0];
			data.triangles[1] = !!triangles[1];
			data.triangles[2] = !!triangles[2];
			data.triangles[3] = !!triangles[3];
			return data;
		}

		public void Load (TriangleBlockData data) {
			for (var i = 0; i < 4; i++){
				if (data.triangles[i])
				{
					CreateTriangle(i);
				}
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

		Mesh GetMesh (int index) 
		{
			return GameObject.Find("PolyEditor").GetComponent<Editor>().meshes[index];
		} 

		void CreateTriangle (int index) {
			var triangle = GenerateTriangleGameObject(this.transform.position);
			var mesh = GetMesh(index);
			triangle.GetComponent<MeshFilter>().mesh = mesh;
			triangle.transform.parent = this.transform;
			triangles[index] = triangle;
		} 

		GameObject GenerateTriangleGameObject (Vector3 position)
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<MeshFilter>();
			gameObject.AddComponent<MeshRenderer>();
			gameObject.transform.position = position;
			return gameObject;
		}
	}
}

