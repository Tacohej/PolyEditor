using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEditor
{
	[System.Serializable]
	public struct LayerData {
		public string name;
		public float zPosition;
		public float parallaxWeight;
		public bool collidable;
		public Material material;
		public Vector2 gridSize; // todo: replace with index in datablocks
		public TriangleBlockData[] triangleDataBlocks;
	}

	public class Layer : MonoBehaviour
	{
		public Vector2Int gridSize;
		public Vector2 spacing;
		public Material material;
		public TriangleBlock triangleBlockPrefab;
		public float zPosition = 0; // todo: set properly
		public float parallaxWeight;
		public bool collidable;
		private TriangleBlock[,] triangleBlocks;

		void Start ()
		{
			triangleBlocks = new TriangleBlock[gridSize.x,gridSize.y];
			GenerateGrid();
		}

		public void AddClosestTriangle (Vector3 position, Mesh[] triangleMeshes)
		{
			position = new Vector3(position.x, position.y, zPosition);
			var triangleBlock = GetClosestBlock(position);
			var location = GetTriangleLocationFromPositions(triangleBlock.transform.position, position);
			triangleBlock.AddTriangle(location, triangleMeshes);
		}

		public void RemoveClosestTriangle (Vector3 position) 
		{
			position = new Vector3(position.x, position.y, zPosition);
			var triangleBlock = GetClosestBlock(position);
			var location = GetTriangleLocationFromPositions(triangleBlock.transform.position, position);
			triangleBlock.RemoveTriangle((int)location);
		}
		
		public LayerData ToData ()
		{
			LayerData layerData = new LayerData();
			layerData.name = name;
			layerData.zPosition = zPosition;
			layerData.parallaxWeight = parallaxWeight;
			layerData.material = material;
			layerData.gridSize = gridSize;
			layerData.triangleDataBlocks = new TriangleBlockData[gridSize.x * gridSize.y];
			for (var y = 0; y < gridSize.y; y++)
			{
				for (var x = 0; x < gridSize.x; x++) 
				{
					layerData.triangleDataBlocks[x * gridSize.x + y] = triangleBlocks[x,y].ToData();
				}
			}
			return layerData;
		}

		void GenerateGrid ()
		{
			for (var y = 0; y < gridSize.y; y++)
			{
				for (var x = 0; x < gridSize.x; x++)
				{
					Vector3 position = new Vector3(x - gridSize.x/2,y - gridSize.y/2,0);
					var instance = Instantiate<TriangleBlock>(triangleBlockPrefab, position, this.transform.rotation);
					instance.transform.parent = gameObject.transform;
					triangleBlocks[x, y] = instance;
				}
			}
		}

		float GetAngleDegFromPoints (Vector3 center, Vector3 target)
		{
			Vector2 dir = target - center;
			float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
			return angle < 0 ? angle + 360 : angle;
		}

		TriangleBlock GetClosestBlock(Vector3 position)
		{
			TriangleBlock closestBlock = triangleBlocks[0,0];
			float closestDistance = float.MaxValue;

			for (var y = 0; y < gridSize.y; y++)
			{
				for (var x = 0; x < gridSize.x; x++)
				{
					var block = triangleBlocks[x,y];
					var blockPos = block.transform.position;
					var dist = Mathf.Abs((position - blockPos).sqrMagnitude);
					if (dist < closestDistance)
					{
						closestDistance = dist;
						closestBlock = block;
					}
				}
			}
			return closestBlock;
		}

		TriangleLocation GetTriangleLocationFromPositions (Vector3 center, Vector3 target)
		{
			var angle = GetAngleDegFromPoints(center, target);
			if (angle > 45 && angle < 135)
				return TriangleLocation.UP;
			else if (angle > 45 && angle < 225)
				return TriangleLocation.LEFT;
			else if (angle > 45 && angle < 315)
				return TriangleLocation.DOWN;
			else
				return TriangleLocation.RIGHT;
		}
	}
}
