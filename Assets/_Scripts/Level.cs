using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEditor
{
	public class LevelData : ScriptableObject
	{
		public LayerData[] layers;
	}
	public class Level : MonoBehaviour
	{
		public List<Layer> layers;
		public Layer layerPrefab;
		private Layer currentLayer;

		void Start ()
		{
			layers = new List<Layer>();
		}

		public Layer GetCurrentLayer ()
		{
			return currentLayer;
		}

		public void CreateNewLayer ()
		{
			Layer layer = Instantiate(layerPrefab, this.transform.position, this.transform.rotation);
			layer.transform.parent = this.transform;
			layer.Create();

			currentLayer = layer; // todo: create activateLayer with z position and alpha
			layers.Add(layer);
		}

		public LevelData ToAsset ()
		{
			LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
			levelData.layers = new LayerData[layers.Count];

			for (var i = 0; i < layers.Count; i++)
			{
				levelData.layers[i] = layers[i].ToData();
			}

			return levelData;
		}
	}
}

