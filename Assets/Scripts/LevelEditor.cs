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
	private int size = 5;

	void Start ()
	{
		mainCamera = Camera.main;
		triangleBlocks = new List<GameObject>();
		levelRoot = new GameObject();
		levelRoot.name = "levelLayer";
		GenerateGrid(size);
		meshes = MeshUtility.GenerateMeshes();
	}

	void Update ()
	{
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
			SaveAsLayer();
		}
	}

	void HandleLeftMouseButton (Vector3 mousePos)
	{
		var closestBlock =  GetClosestBlock(mousePos, triangleBlocks);
		float angle = GetAngleDegFromPoints(closestBlock.transform.position, mousePos);
		TriangleLocation location = GetTriangleLocationFromAngle(angle);

		var hasTriangle = closestBlock.GetComponent<TriangleBlock>().triangles[(int)location];
		if (!hasTriangle)
		{
			var gameObject = GenerateTriangleGameObject(closestBlock.transform.position);
			gameObject.GetComponent<MeshFilter>().mesh = meshes[(int)location];
			closestBlock.GetComponent<TriangleBlock>().triangles[(int)location] = gameObject;
			gameObject.transform.parent = closestBlock.transform;
		}
	}

	void HandleRightMouseButton (Vector3 mousePos)
	{
		var closestBlock =  GetClosestBlock(mousePos, triangleBlocks);
		float angle = GetAngleDegFromPoints(closestBlock.transform.position, mousePos);
		TriangleLocation location = GetTriangleLocationFromAngle(angle);

		var hasTriangle = closestBlock.GetComponent<TriangleBlock>().triangles[(int)location];
		if (hasTriangle)
		{
			var gameObject = closestBlock.GetComponent<TriangleBlock>().triangles[(int)location];
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
		int index = 0;

		for (var i = 0; i < blocks.Count; i++)
		{
			var blockPos = blocks[i].transform.position;
			var dist = Mathf.Abs((position - blockPos).sqrMagnitude);
			if (dist < closestDistance) 
			{
				closestDistance = dist;
				closestBlock = blocks[i];
				index = i;
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

	void SaveAsLayer ()
	{
		Layer layer = ScriptableObject.CreateInstance<Layer>();
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
		AssetDatabase.CreateAsset(layer, "Assets/ScriptableObjects.asset");
		print("Asset Created");
	}




	// void SaveAsLayer () {

	// 	Layer layer = ScriptableObject.CreateInstance<Layer>();
	// 	List<BlockStruct> blockStructs = new List<BlockStruct>();

	// 	for (var i = 0; i < blocks.Count; i++)
	// 	{
	// 		var block = blocks[i];
	// 		if (block.top || block.right || block.bottom || block.left)
	// 		{	
	// 			BlockStruct blockStruct = new BlockStruct();

	// 			blockStruct.position = block.position;
	// 			blockStruct.top = block.top;
	// 			blockStruct.right = block.right;
	// 			blockStruct.bottom = block.bottom;
	// 			blockStruct.left = block.left;
				
	// 			blockStructs.Add(blockStruct);
	// 		}
	// 	}

	// 	layer.blocks = blockStructs.ToArray();
	// 	AssetDatabase.CreateAsset(layer, "Assets/ScriptableObjects.asset");

	// }

}
