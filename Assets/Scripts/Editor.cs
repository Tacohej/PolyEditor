using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

namespace PolyEditor
{
	[RequireComponent(typeof(UIControls))]
	public class Editor : MonoBehaviour {

		// Public
		public LevelData levelData; // used temp as level to load / export as prefab
		public Level levelPrefab;

		// Private
		private Level level;
		private Camera mainCamera;
		private UIControls uiControls;
		private Mesh[] triangleMeshes;
		private Layer currentLayer;
		private string rootPath = "Assets/Generated";

		void Start ()
		{
			mainCamera = Camera.main;
			triangleMeshes = MeshUtility.GenerateMeshes();
			uiControls = GetComponent<UIControls>();
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
				currentLayer.AddClosestTriangle(mousePos);
			}

			if (Input.GetMouseButton(1))
			{
				currentLayer.RemoveClosestTriangle(mousePos);
			}

			if (Input.GetKeyDown(KeyCode.Tab))
			{
				IncrementCurrentLayer();
			}
		}

		public void IncrementCurrentLayer () {

			var layers = level.GetLayers();

			if (layers.Count == 0) { return; }

			for (var i = 0; i <  layers.Count; i++)
			{
				if (layers[i] == currentLayer){
					var newIndex = (i + 1) % layers.Count;
					SetCurrentLayer(level.GetLayers()[newIndex]);
					return;
				}
			}
		}

		public Mesh[] GetTriangleMeshes() 
		{
			return triangleMeshes;
		}

		public void AddNewLayer () 
		{
			if (!level)
			{
				level = Instantiate(levelPrefab);
				level.transform.parent = this.transform;
			}
			var layer = level.AddNewLayer();
			SetCurrentLayer(layer);
		}

		public void SetParallaxWeightCurrentLayer (float newValue)
		{
			currentLayer.SetParallaxWeight(newValue);
		}

		public void SetZPositionCurrentLayer (float newValue)
		{
			currentLayer.SetZPosition(newValue);
		}

		public void SaveLevelAsset ()
		{
			AssetDatabase.CreateAsset(level.ToAsset(), GetValidPath("/Projects/level", ".asset"));
		}

		public void LoadLevelAsset () 
		{
			level = Instantiate(levelPrefab, this.transform.position, this.transform.rotation);
			level.transform.parent = this.transform;
			level.Load(levelData);
		}

		public void CreateLevelPrefab ()
		{
			GameObject levelRoot = new GameObject();
			levelRoot.name = levelData.name;

			for(var i = 0; i < levelData.layers.Length; i++)
			{
				var layerData = levelData.layers[i];
				var layerRoot = new GameObject();
				layerRoot.transform.parent = levelRoot.transform;

				var parallax = layerRoot.AddComponent<ParallaxLayer>();
				parallax.name = layerData.name;
				parallax.parallaxWeight = layerData.parallaxWeight;
				parallax.zPosition = layerData.zPosition;

				var meshFilter = layerRoot.AddComponent<MeshFilter>();
				meshFilter.mesh = new Mesh();
				meshFilter.mesh.CombineMeshes(CreateCombinedMeshes(layerData, layerRoot));
				
				layerRoot.AddComponent<MeshRenderer>();
				
				if (layerData.isCollidable)
				{
					AddColliderToLayer(layerRoot, layerData.triangleDataBlocks);
				}

				AssetDatabase.CreateAsset(meshFilter.mesh , GetValidPath("/Prefabs/mesh", ".asset"));
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

			}

			PrefabUtility.CreatePrefab(GetValidPath("/Prefabs/level", ".prefab"), levelRoot);
			DestroyImmediate(levelRoot);
		}

		void SetCurrentLayer (Layer layer)
		{
			currentLayer = layer;
			uiControls.parallaxWeightSlider.value = layer.GetParallaxWeight();
			uiControls.zPositionSlider.value = layer.GetZPosition();
		}

		void AddColliderToLayer (GameObject layer, TriangleBlockData[] triangleBlocks) {
			var index = 0;
			var collider = layer.AddComponent<PolygonCollider2D>();
			collider.pathCount = 0;

			for (var i = 0; i < triangleBlocks.Length; i++)
			{
				var triangleBlock = triangleBlocks[i];
				for (var j = 0; j < triangleBlock.triangles.Length; j++)
				{
					var triangle = triangleBlock.triangles[j];
					if (triangle)
					{
						collider.SetPath(index, Vector3DArrayTo2D(GetTriangleMeshes()[j].vertices, triangleBlock.position));
						collider.pathCount++;
						index++;
					}
				}
			}
		}

		Vector2[] Vector3DArrayTo2D (Vector3[] vec3s, Vector3 offset)
		{
			Vector2[] vec2s = new Vector2[vec3s.Length];
			for (var i = 0; i < vec3s.Length; i++)
			{
				vec2s[i] = (Vector2)(vec3s[i] + offset);
			}
			return vec2s;
		}

		CombineInstance[] CreateCombinedMeshes (LayerData layerData, GameObject root)
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
						combine.mesh = triangleMeshes[j];
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

		string GetValidPath (string path, string fileEnding)
		{
			string validPath = rootPath + path + fileEnding;
			int postFix = 0;

			while (System.IO.File.Exists(validPath))
			{
				validPath = validPath.Replace(fileEnding, "_" + postFix.ToString() + fileEnding);
				postFix++;
			}

			print ("path: " + validPath);
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

