using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PolyEditor
{
	public class Editor : MonoBehaviour {
		public Level level;
		private Camera mainCamera;
		private Mesh[] meshes;

		void Start ()
		{
			mainCamera = Camera.main;
			meshes = MeshUtility.GenerateMeshes();
		}
		void Update()
		{
			var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

			if (Input.GetMouseButton(0))
			{
				level.currentLayer.AddClosestTriangle(mousePos, meshes);
			}

			if (Input.GetMouseButton(1))
			{
				level.currentLayer.RemoveClosestTriangle(mousePos);
			}

			if (Input.GetKeyDown(KeyCode.Space)) // todo: fix temp
			{
				CreateLevelAsset();
			}
		}

		void CreateLevelAsset ()
		{
			AssetDatabase.CreateAsset(level.ToAsset(), GetValidPath());
		}

		string GetValidPath (string path = "Assets/ScriptableObjects/Generated/SavedLevels/level.asset")
		{
			string validPath = path;
			int postFix = 0;

			while (System.IO.File.Exists(validPath))
			{
				validPath = path.Replace(".asset", "_" + postFix.ToString() + ".asset");
				postFix++;
			}

			return validPath;
		}
	}
}

