using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEditor
{
	[RequireComponent(typeof(ParallaxLayer))]
	public class Layer : MonoBehaviour
	{
		// Public
		public Vector2Int gridSize;
		public TriangleBlock triangleBlockPrefab;

		// Private
		private TriangleBlock[,] triangleBlocks;
		private ParallaxLayer parallaxComp;

		public void Awake() 
		{
			parallaxComp = GetComponent<ParallaxLayer>();
		}

		public void SetParallaxWeight (float newValue) 
		{
			parallaxComp.parallaxWeight = newValue;
		}

		public float GetParallaxWeight ()
		{
			return parallaxComp.parallaxWeight;
		}

		public void SetZPosition (float newValue)
		{
			parallaxComp.zPosition = newValue;
		}

		public float GetZPosition ()
		{
			return parallaxComp.zPosition;
		}

		public void Create ()
		{
			triangleBlocks = new TriangleBlock[gridSize.x,gridSize.y];
			GenerateGrid();
		}

		public void Load (LayerData layer)
		{
			gridSize = layer.gridSize;
			parallaxComp.zPosition = layer.zPosition;
			parallaxComp.parallaxWeight = layer.parallaxWeight;
			triangleBlocks = new TriangleBlock[gridSize.x, gridSize.y];
			GenerateGrid();

			for (var y = 0; y < gridSize.y; y++)
			{
				for (var x = 0; x < gridSize.x; x++)
				{
					triangleBlocks[x,y].Load(layer.triangleDataBlocks[x * gridSize.x + y]);
				}
			}
		}

		public void AddClosestTriangle (Vector3 position)
		{
			position = new Vector3(position.x, position.y, 0);
			var triangleBlock = GetClosestBlock(position);
			var location = GetTriangleLocationFromPositions(triangleBlock.transform.position, position);
			triangleBlock.AddTriangle(location);
		}

		public void RemoveClosestTriangle (Vector3 position) 
		{
			position = new Vector3(position.x, position.y, 0);
			var triangleBlock = GetClosestBlock(position);
			var location = GetTriangleLocationFromPositions(triangleBlock.transform.position, position);
			triangleBlock.RemoveTriangle((int)location);
		}
		
		public LayerData ToData ()
		{
			var	parallaxComp = GetComponent<ParallaxLayer>();
			var layerData = new LayerData();
			layerData.name = name;
			layerData.zPosition = parallaxComp.zPosition;
			layerData.parallaxWeight = parallaxComp.parallaxWeight;
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
					float xPos = x - (gridSize.x - 1) * 0.5f;
					float yPos = y - (gridSize.y - 1) * 0.5f;
					Vector3 position = new Vector3(xPos, yPos, 0);
					var instance = Instantiate<TriangleBlock>(triangleBlockPrefab, position, this.transform.rotation);
					instance.transform.parent = gameObject.transform;
					triangleBlocks[x, y] = instance;
				}
			}
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

		TriangleLocation GetTriangleLocationFromPositions(Vector3 center, Vector3 target)
		{
			var dir = target - center;
			if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
			{
				return dir.x > 0 
					? TriangleLocation.RIGHT 
					: TriangleLocation.LEFT;
			} 
				else
			{
				return dir.y > 0 
					? TriangleLocation.UP
					: TriangleLocation.DOWN;
			}
		}
	}
}
