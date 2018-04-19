using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditor : MonoBehaviour 
{
	private List<GameObject> triangleBlocks;
	private GameObject levelRoot;
	private Camera mainCamera;
	private Mesh[] meshes;
	private int size = 10;
	public Level testLevel;

	void Start ()
	{
		mainCamera = Camera.main;
		meshes = MeshUtility.GenerateMeshes();
		triangleBlocks = new List<GameObject>();
		levelRoot = new GameObject();
		levelRoot.name = "levelLayer";
		GenerateGrid(size);
	}

	void Update ()
	{
		// todo: break out to camera controller
		var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

		if (Input.GetMouseButton(0))
		{
			HandleLeftMouseButton(mousePos);
		}

		if (Input.GetMouseButton(1))
		{
			HandleRightMouseButton(mousePos);
		}
	}

	void HandleLeftMouseButton (Vector3 mousePos)
	{
		var closestBlock =  GetClosestBlock(mousePos, triangleBlocks);
		float angle = GetAngleDegFromPoints(closestBlock.transform.position, mousePos);
		TriangleLocation location = GetTriangleLocationFromAngle(angle);
		int triangleLocation = (int)location;

		var hasTriangle = closestBlock.GetComponent<TriangleBlock>().triangles[triangleLocation];
		if (!hasTriangle)
		{
			var gameObject = GenerateTriangleGameObject(closestBlock.transform.position);
			gameObject.GetComponent<MeshFilter>().mesh = meshes[triangleLocation];
			gameObject.transform.parent = closestBlock.transform;
			gameObject.name = location.ToString();
			
			closestBlock.GetComponent<TriangleBlock>().triangles[triangleLocation] = gameObject;
		}
	}

	void HandleRightMouseButton (Vector3 mousePos)
	{
		var closestBlock =  GetClosestBlock(mousePos, triangleBlocks);
		float angle = GetAngleDegFromPoints(closestBlock.transform.position, mousePos);
		TriangleLocation location = GetTriangleLocationFromAngle(angle);
		int triangleLocation = (int)location;

		var hasTriangle = closestBlock.GetComponent<TriangleBlock>().triangles[triangleLocation];
		if (hasTriangle)
		{
			var gameObject = closestBlock.GetComponent<TriangleBlock>().triangles[triangleLocation];
			if (gameObject)
			{
				DestroyGameObject(gameObject);
			}
		}
	}

	GameObject GetClosestBlock (Vector3 position, List<GameObject> blocks) 
	{
		float closestDistance = float.MaxValue;
		GameObject closestBlock = blocks[0]; // todo: temp solution

		for (var i = 0; i < blocks.Count; i++)
		{
			var blockPos = blocks[i].transform.position;
			var dist = Mathf.Abs((position - blockPos).sqrMagnitude);
			if (dist < closestDistance) 
			{
				closestDistance = dist;
				closestBlock = blocks[i];
			}
		}
		return closestBlock;
	}

	void DestroyGameObject (GameObject gameObject)
	{
		gameObject.transform.parent = null;
		Destroy(gameObject);
		gameObject = null;
	}

	float GetAngleDegFromPoints (Vector3 center, Vector3 target)
	{
		Vector2 dir = target - center;
		float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		return angle < 0 ? angle + 360 : angle;
	}

	TriangleLocation GetTriangleLocationFromAngle (float angle)
	{
		if (angle > 45 && angle < 135)
		{
			return TriangleLocation.UP;
		}
		else if (angle > 45 && angle < 225)
		{
			return TriangleLocation.LEFT;
		}
		else if (angle > 45 && angle < 315)
		{
			return TriangleLocation.DOWN;
		}
		else
			return TriangleLocation.RIGHT;
	}

	void GenerateGrid (int size)
	{
		for (var y = size; y > -size; y--) 
		{
			for (var x = -size; x < size; x++) 
			{
				var instance = new GameObject();
				instance.AddComponent<TriangleBlock>();
				instance.transform.position = new Vector3(x,y,0);
				instance.transform.parent = levelRoot.transform;
				triangleBlocks.Add(instance);
			}
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

	public void SaveLevel () 
	{
		// todo: save more than one layer
		SaveLayer();
	}

	public void LoadLevel ()
	{
		for(var i = 0; i < testLevel.layers.Count; i++) {
			LoadLayer(testLevel.layers[i]);
		}
	}

	public void SaveLayer ()
	{
		LevelLayer layer = ScriptableObject.CreateInstance<LevelLayer>();
		List<TriangleBlock.Struct> structs = new List<TriangleBlock.Struct>();

		for (var i = 0; i < triangleBlocks.Count; i++)
		{
			var block = triangleBlocks[i];
			var triangleBlock = block.GetComponent<TriangleBlock>();

			if (triangleBlock.hasTriangles())
			{	
				structs.Add(triangleBlock.GenerateStruct());
			}
		}

		layer.dataStructs = structs.ToArray();
		AssetDatabase.CreateAsset(layer, GetValidPath());
		print("Asset Created");
	}

	void LoadLayer (LevelLayer layer)
	{
		var root = new GameObject();
		root.AddComponent<ParallaxLayer>();
		root.name = layer.name;

		var paraLayer = root.GetComponent<ParallaxLayer>();
		paraLayer.SetSortOrder(layer.sortOrder);
		paraLayer.SetParallaxWeight(layer.parallaxWeight);
		paraLayer.SetCamera(mainCamera);

		for (var i = 0; i < layer.dataStructs.Length; i++)
		{
			var currTriangle = layer.dataStructs[i];
			for (var j = 0; j < currTriangle.triangles.Length; j++)
			{
				if (currTriangle.triangles[j]) {
					var gameObject = GenerateTriangleGameObject(currTriangle.position);
					gameObject.GetComponent<MeshFilter>().mesh = meshes[j];
					gameObject.GetComponent<MeshRenderer>().material = layer.material;
					gameObject.transform.parent = root.transform;
				}
			}
		}
	}

	void LoadLayerAsOneMesh (LevelLayer layer)
	{
		var root = new GameObject();
		root.AddComponent<ParallaxLayer>();
		root.AddComponent<MeshFilter>();
		root.AddComponent<MeshRenderer>();
		root.name = layer.name;

		var paraLayer = root.GetComponent<ParallaxLayer>();
		paraLayer.SetSortOrder(layer.sortOrder);
		paraLayer.SetParallaxWeight(layer.parallaxWeight);
		paraLayer.SetCamera(mainCamera);

		List<CombineInstance> combines = new List<CombineInstance>();

		for (var i = 0; i < layer.dataStructs.Length; i++)
		{
			var currTriangle = layer.dataStructs[i];

			for (var j = 0; j < currTriangle.triangles.Length; j++)
			{
				if (currTriangle.triangles[j]) {

					CombineInstance combine = new CombineInstance();
					combine.mesh = meshes[j];
					combine.transform = new Matrix4x4(
						new Vector4(1, 0, 0, 0),
						new Vector4(0, 1, 0, 0),
						new Vector4(0, 0, 1, 0),
						new Vector4(currTriangle.position.x, currTriangle.position.y, 0, 1)
					);
					combines.Add(combine);
				}
			}
		}

		root.GetComponent<MeshFilter>().mesh = new Mesh();
		root.GetComponent<MeshFilter>().mesh.CombineMeshes(combines.ToArray());
		root.GetComponent<MeshRenderer>().material = layer.material;

		if (layer.collidable)
		{
			AddPolygonCollider(root, layer.dataStructs);
		}
	}

	void AddPolygonCollider (GameObject root, TriangleBlock.Struct[] triangleStructs) 
	{
		var index = 0;
		PolygonCollider2D polyCollider = root.AddComponent<PolygonCollider2D>();
		polyCollider.pathCount = 0;
		for (var i = 0; i < triangleStructs.Length; i++)
		{
			var triangleStruct = triangleStructs[i];
			for (var j = 0; j < triangleStruct.triangles.Length; j++)
			{
				if (triangleStruct.triangles[j])
				{
					polyCollider.SetPath(index, Vector3ToVector2(meshes[j].vertices, triangleStruct.position));
					polyCollider.pathCount++;
					index++;
				}
			}
		}
	}

	Vector2[] Vector3ToVector2 (Vector3[] vec3, Vector3 offset)
	{
		Vector2[] vec2 = new Vector2[vec3.Length];
		for (var i = 0; i < vec3.Length; i++)
		{
			vec2[i] = (Vector2)(vec3[i] + offset);
		}
		return vec2;
	}

	string GetValidPath (string path = "Assets/ScriptableObjects/Generated/layer_data.asset")
	{
		string validPath = path;
		int postFix = 0;

		while (System.IO.File.Exists(validPath)) {
			validPath = path.Replace(".asset", "_" + postFix.ToString() + ".asset");
			postFix++;
		}

		return validPath;
	}
}
