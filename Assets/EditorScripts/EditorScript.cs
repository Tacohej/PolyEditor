using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelEditor))]
public class EditorScript : Editor {

	public override void OnInspectorGUI()
	{

		DrawDefaultInspector();

		LevelEditor levelEditor = (LevelEditor)target;

		if(GUILayout.Button("Load Level"))
		{
			levelEditor.LoadLevel();
		}

		if (GUILayout.Button("Save Level"))
		{
			levelEditor.SaveLevel();
		}


	}


}
