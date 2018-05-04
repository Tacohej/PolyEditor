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
		
		[HideInInspector]
		public Layer currentLayer;


		void Start () {
			// layers = new List<Layer>();
			currentLayer = layers[0];
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

