using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PolyEditor {
	public static class MeshUtility
	{
		private const float z_value = 0;

		public static Mesh[] GenerateMeshes ()
		{
			var size = .5f;
			Vector3[] top = new Vector3[3] {
				new Vector3(0, 0, z_value),
				new Vector3(-size, size, z_value),
				new Vector3(size, size, z_value)
			};
			Vector3[] left = new Vector3[3] {
				new Vector3(0, 0, z_value),
				new Vector3(-size, -size, z_value),
				new Vector3(-size, size, z_value)
			};
			Vector3[] bottom = new Vector3[3] {
				new Vector3(0, 0, z_value),
				new Vector3(size, -size, z_value),
				new Vector3(-size, -size, z_value)
			};
			Vector3[] right = new Vector3[3] {
				new Vector3(0, 0, z_value),
				new Vector3(size, size, z_value),
				new Vector3(size, -size, z_value)
			};

			return new Mesh[4] {
				GenerateTriangleMesh(top),
				GenerateTriangleMesh(left),
				GenerateTriangleMesh(bottom),
				GenerateTriangleMesh(right)
			};
		}
		private static Mesh GenerateTriangleMesh (Vector3[] vertices)
		{
			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = new int[] {0, 1, 2};
			mesh.uv = new Vector2[]
			{
				new Vector2(0,0),
				new Vector2(0,1),
				new Vector2(1,1)
			};

			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			return mesh;
		}
	}

}
