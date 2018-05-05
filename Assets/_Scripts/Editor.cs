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
		
		[HideInInspector]
		public Mesh[] meshes;

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

			if (IsPointerOverUIObject() || level == null)
			{
				return;
			}

			if (Input.GetMouseButton(0))
			{
				level.GetCurrentLayer().AddClosestTriangle(mousePos);
			}

			if (Input.GetMouseButton(1))
			{
				level.GetCurrentLayer().RemoveClosestTriangle(mousePos);
			}

		}

		public void AddNewLayer () 
		{
			if (!level)
			{
				level = Instantiate(levelPrefab);
			}
			level.AddNewLayer();
		}

		public void SaveLevelAsset ()
		{
			AssetDatabase.CreateAsset(level.ToAsset(), GetValidPath());
		}

		public void LoadLevelAsset () 
		{
			level = Instantiate(levelPrefab, this.transform.position, this.transform.rotation);
			level.Load(levelData);
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

