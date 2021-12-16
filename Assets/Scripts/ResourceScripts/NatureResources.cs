using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NatureResources
{
	// Trees
	public static GameObject treeBirch = Resources.Load<GameObject> ("WorldObjects/Trees/TreeBirch");
	public static GameObject treeBlue = Resources.Load<GameObject> ("WorldObjects/Trees/TreeBlue");
	public static GameObject treeFig = Resources.Load<GameObject> ("WorldObjects/Trees/TreeFig");
	public static GameObject treeForest1 = Resources.Load<GameObject> ("WorldObjects/Trees/TreeForest1");
	public static GameObject treeForest2 = Resources.Load<GameObject> ("WorldObjects/Trees/TreeForest2");
	public static GameObject treeJoshua = Resources.Load<GameObject> ("WorldObjects/Trees/TreeJoshua");
	public static GameObject treeOak = Resources.Load<GameObject> ("WorldObjects/Trees/TreeOak");
	public static GameObject treePalm = Resources.Load<GameObject> ("WorldObjects/Trees/TreePalm");
	public static GameObject treePine = Resources.Load<GameObject> ("WorldObjects/Trees/TreePine");
	public static GameObject treePineSmall = Resources.Load<GameObject> ("WorldObjects/Trees/TreePineSmall");
	public static GameObject treeRainforest1 = Resources.Load<GameObject> ("WorldObjects/Trees/TreeRainforest1");
	public static GameObject treeRed = Resources.Load<GameObject> ("WorldObjects/Trees/TreeRed");
	public static GameObject treeWhite = Resources.Load<GameObject>("WorldObjects/Trees/TreeWhite");
	public static GameObject treeYellow = Resources.Load<GameObject> ("WorldObjects/Trees/TreeYellow");

	// used only for null checking - can delete later
	public static GameObject[] objects = { treeBirch, treeBlue, treeFig, treeForest1, treeForest2,
  treeJoshua, treeOak, treePalm, treePine, treePineSmall, treeRainforest1, treeRed, treeWhite,
  treeYellow};

	//"shrubs"
	public static GameObject wheat = Resources.Load<GameObject> ("WorldObjects/Shrubs/Shrub7");
	public static GameObject cane = Resources.Load<GameObject> ("WorldObjects/Shrubs/Shrub9");
	public static GameObject stick = Resources.Load<GameObject> ("WorldObjects/Shrubs/Shrub5");
	public static GameObject dead_bush = Resources.Load<GameObject> ("WorldObjects/Shrubs/Shrub3");
	public static GameObject bush1 = Resources.Load<GameObject> ("WorldObjects/Shrubs/Shrub11");
	public static GameObject bush2 = Resources.Load<GameObject> ("WorldObjects/Shrubs/Shrub12");
	public static GameObject fern1 = Resources.Load<GameObject> ("WorldObjects/Shrubs/Shrub13");
	public static GameObject cactus = Resources.Load<GameObject> ("WorldObjects/Shrubs/Shrub14");
	public static GameObject rock1 = Resources.Load<GameObject> ("WorldObjects/Rocks/Boulder1");
	public static GameObject rock2 = Resources.Load<GameObject> ("WorldObjects/Rocks/Boulder2");
}
