using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

namespace PolyEditor
{
	[RequireComponent(typeof(UIControls))]
	public class Editor : MonoBehaviour {
		public Level levelPrefab;
		private Level level;
		private Camera mainCamera;
		private UIControls uiControls;
		
		[HideInInspector]
		public Mesh[] meshes;

		// todo: temp
		public LevelData levelData;

		void Start ()
		{
			mainCamera = Camera.main;
			meshes = MeshUtility.GenerateMeshes();
			uiControls = GetComponent<UIControls>();
		}
		void Update()
		{
			var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

			if (Input.GetKeyDown(KeyCode.P))
			{
				CreateLevelPrefab(levelData);
			}

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
				level.transform.parent = this.transform;
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
			level.transform.parent = this.transform;
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

		public void CreateLevelPrefab (LevelData data) // todo: move to other script?
		{
			GameObject levelRoot = new GameObject();
			levelRoot.name = data.name;

			for(var i = 0; i < data.layers.Length; i++)
			{
				var layerData = data.layers[i];
				var layerRoot = new GameObject();
				layerRoot.transform.parent = levelRoot.transform;

				var parallax = layerRoot.AddComponent<ParallaxLayer>();
				parallax.name = layerData.name;
				parallax.parallaxWeight = layerData.parallaxWeight;
				parallax.zPosition = layerData.zPosition;

				var meshFilter = layerRoot.AddComponent<MeshFilter>();
				meshFilter.mesh = new Mesh();
				meshFilter.mesh.CombineMeshes(CreateCombinedMeshes(layerData, layerRoot));

				AssetDatabase.CreateAsset(meshFilter.mesh , "Assets/GeneratedLevels/mesh" + i + ".asset");
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				layerRoot.AddComponent<MeshRenderer>();
			}

			PrefabUtility.CreatePrefab("Assets/GeneratedLevels/level.prefab", levelRoot);
			DestroyImmediate(levelRoot);
			print("Prefab generated");
		}

		CombineInstance[] CreateCombinedMeshes (LayerData layerData, GameObject root)  // todo: move to other script?
		{
			List<CombineInstance> combines = new List<CombineInstance>();
			for (var i = 0; i < layerData.triangleDataBlocks.Length; i++)
			{
				var triangleBlockData = layerData.triangleDataBlocks[i];
				for (var j = 0; j < triangleBlockData.triangles.Length; j++)
				{
					if (triangleBlockData.triangles[j])
					{
						CombineInstance combine = new CombineInstance();
						combine.mesh = meshes[j];
						combine.transform = Create2DTransformMatrix(triangleBlockData.position);
						combines.Add(combine);
					}
				}
			}
			return combines.ToArray();
		}

		Matrix4x4 Create2DTransformMatrix (Vector3 position) 
		{
			return new Matrix4x4(
				new Vector4(1, 0, 0, 0),
				new Vector4(0, 1, 0, 0),
				new Vector4(0, 0, 1, 0),
				new Vector4(position.x, position.y, 0, 1)
			);
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

