﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEditor
{
	public enum TriangleLocation { UP, LEFT, DOWN, RIGHT }
	public class TriangleBlock : MonoBehaviour
	{
		// Private
		private GameObject[] triangles = new GameObject[4];

		public void AddTriangle(TriangleLocation location, Material material)
		{
			var index = (int)location;

			if (!triangles[index])
			{
				CreateTriangle(index, material);
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
		public void Load (TriangleBlockData data, Material material) {
			for (var i = 0; i < 4; i++){
				if (data.triangles[i])
				{
					CreateTriangle(i, material);
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
		void CreateTriangle (int index, Material material) {
			var triangle = GenerateTriangleGameObject(this.transform.position);
			var mesh = GetMesh(index);
			triangle.GetComponent<MeshFilter>().mesh = mesh;
			triangle.GetComponent<MeshRenderer>().material = material;
			triangle.transform.parent = this.transform;
			triangles[index] = triangle;
		}
		Mesh GetMesh (int index) 
		{
			return GameObject.Find("PolyEditor").GetComponent<Editor>().GetTriangleMeshes()[index];
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

