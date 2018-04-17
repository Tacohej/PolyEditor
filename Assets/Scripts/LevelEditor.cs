using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LevelEditor : MonoBehaviour 
{
	public GameObject triangleBlockPrefab;
	private List<GameObject> triangleBlocks;
	private GameObject levelRoot;
	private Camera mainCamera;
	private Mesh[] meshes;
	private int size = 10;

	// todo: temp
	public Level testLevel;

	void Start ()
	{

		mainCamera = Camera.main;
		meshes = MeshUtility.GenerateMeshes();

		// todo: temp
		if (testLevel) {

			for(var i = 0; i < testLevel.layers.Count; i++) {
				LoadLayer(testLevel.layers[i]);
			}
			return;
		}

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

		if (Input.GetKeyDown(KeyCode.Space))
		{
			SaveLayer();
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
				var instance = Instantiate(triangleBlockPrefab, new Vector3(x,y,0), Quaternion.identity) as GameObject;
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

	void SaveLayer ()
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

		layer.triangleStructs = structs.ToArray();
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

		for (var i = 0; i < layer.triangleStructs.Length; i++)
		{
			var currTriangle = layer.triangleStructs[i];

			//var instance = Instantiate(triangleBlockPrefab, currTriangle.position, Quaternion.identity) as GameObject;
			for (var j = 0; j < currTriangle.triangles.Length; j++)
			{
				if (currTriangle.triangles[j]) {
					var gameObject = GenerateTriangleGameObject(currTriangle.position);
					gameObject.GetComponent<MeshFilter>().mesh = meshes[j];
					gameObject.GetComponent<MeshRenderer>().material = layer.material;
					gameObject.transform.parent = root.transform;

					//root.transform.parent = instance.transform;
					//currTriangle.triangles[j] = gameObject;
				}
			}
		}
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
