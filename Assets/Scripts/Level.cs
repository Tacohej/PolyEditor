using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEditor
{
	public class Level : MonoBehaviour
	{
		// Public
		public Layer layerPrefab;
		
		// Private
		private List<Layer> layers;

		void Awake()
		{
			layers = new List<Layer>();
		}

		public List<Layer> GetLayers()
		{
			return layers;
		}

		public Layer AddNewLayer ()
		{
			Layer layer = Instantiate(layerPrefab, this.transform.position, this.transform.rotation);
			layer.transform.parent = this.transform;
			layer.Create();
			layers.Add(layer);
			return layer;
		}

		public void Load (LevelData data)
		{
			for (var i = 0; i < data.layers.Length; i++) 
			{
				var layerInstance = Instantiate(layerPrefab, transform.position, transform.rotation);
				layerInstance.transform.parent = this.transform;
				layerInstance.Load(data.layers[i]);
			}
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

