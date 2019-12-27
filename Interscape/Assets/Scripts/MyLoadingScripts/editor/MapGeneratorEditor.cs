using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator1))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
	{
		MapGenerator1 mapGen = (MapGenerator1)target;

		DrawDefaultInspector ();

		if (GUILayout.Button ("Generate")) {
			mapGen.GenerateMap ();
		}
	}
}
