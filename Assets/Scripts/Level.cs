using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Level", menuName="LevelEditor/New Level")]
public class Level : ScriptableObject {

	[Space(10)]
	public string levelName = "New level";
	
	[Space(10)]
	[TextArea]
	public string levelDescription = "Probably useful sometimes...";

	[Space(10)]
	public List<LevelLayer> layers = new List<LevelLayer>();

}
