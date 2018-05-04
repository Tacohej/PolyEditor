using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

namespace PolyEditor
{
	public class Editor : MonoBehaviour {
		public Level levelPrefab;
		private Level level;
		private Camera mainCamera;
		private Mesh[] meshes;

		// todo: temp
		public LevelData levelData;

		void Start ()
		{
			mainCamera = Camera.main;
			meshes = MeshUtility.GenerateMeshes();
		}
		void Update()
		{
			var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

			if (IsPointerOverUIObject())
			{
				return;
			}

			if (Input.GetMouseButton(0))
			{
				level.GetCurrentLayer().AddClosestTriangle(mousePos, meshes);
			}

			if (Input.GetMouseButton(1))
			{
				level.GetCurrentLayer().RemoveClosestTriangle(mousePos);
			}

			if (Input.GetKeyDown(KeyCode.L))
			{
				SaveLevelAsset();
			}

			if (Input.GetKeyDown(KeyCode.Space))
			{
				AddNewLayer();
			}

		}

		public void AddNewLayer () 
		{
			if (!level)
			{
				level = Instantiate(levelPrefab);
			}
			level.CreateNewLayer();
		}

		public void SaveLevelAsset ()
		{
			AssetDatabase.CreateAsset(level.ToAsset(), GetValidPath());
		}

		public void LoadLevelAsset () 
		{
			print("todo");
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

		bool IsPointerOverUIObject ()
		{
			PointerEventData evenDataCurrentPosition = new PointerEventData(EventSystem.current);
			evenDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(evenDataCurrentPosition, results);
			return results.Count > 0;
		}
	}
}

