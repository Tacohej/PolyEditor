using UnityEditor;
using UnityEngine;

public class PolyEditor_Temp {

	public static void GenerateGrid (int size)
	{
		for (var y = size; y > -size; y--) 
		{
			for (var x = -size; x < size; x++) 
			{
				var instance = new GameObject();
				instance.AddComponent<TriangleBlock>();
				instance.transform.position = new Vector3(x,y,0);
				// instance.transform.parent = levelRoot.transform;
				// triangleBlocks.Add(instance);
			}
		}
	}

}

public class PolyEditorWindow : EditorWindow {

	int size = 10;

	[MenuItem("PolyEditor/Open")]
	static void Init()
	{
		PolyEditorWindow window = (PolyEditorWindow)EditorWindow.GetWindow(typeof(PolyEditorWindow));
		window.Show();
	}

	void OnGUI()
	{
			if (GUILayout.Button("Generate Grid"))
			{
				PolyEditor_Temp.GenerateGrid(size);
			}
	}

	void Update () {


		// Debug.Log("'Test'");
		
	}

}
