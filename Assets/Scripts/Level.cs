using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Level", menuName="LevelEditor/New Level")]
public class Level : ScriptableObject {
	public string levelName = "New level";
	[TextArea] public string levelDescription = "Probably useful sometimes...";
	public List<LevelLayer> layers = new List<LevelLayer>();

}
