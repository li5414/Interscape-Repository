using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NatureResources
{
	// Trees
	public static GameObject treeBirch = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/TreeBirch");
	public static GameObject treeBlue = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/TreeBlue");
	public static GameObject treeFig = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/TreeFig");
	public static GameObject treeForest1 = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/TreeForest1");
	public static GameObject treeForest2 = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/TreeForest2");
	public static GameObject treeJoshua = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/TreeJoshua");
	public static GameObject treeOak = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/TreeOak");
	public static GameObject treePalm = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/TreePalm");
	public static GameObject treePine = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/TreePine");
	public static GameObject treePineSmall = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/TreePineSmall");
	public static GameObject treeRainforest1 = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/TreeRainforest1");
	public static GameObject treeRed = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/TreeRed");
	public static GameObject treeWhite = Resources.Load<GameObject>("Sprites/Map/TreeFabs/TreeWhite");
	public static GameObject treeYellow = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/TreeYellow");

	// used only for null checking - can delete later
	public static GameObject[] objects = { treeBirch, treeBlue, treeFig, treeForest1, treeForest2,
  treeJoshua, treeOak, treePalm, treePine, treePineSmall, treeRainforest1, treeRed, treeWhite,
  treeYellow};

	//"shrubs"
	public static GameObject wheat = Resources.Load<GameObject> ("Sprites/Map/ShrubFabs/Shrub7");
	public static GameObject cane = Resources.Load<GameObject> ("Sprites/Map/ShrubFabs/Shrub9");
	public static GameObject stick = Resources.Load<GameObject> ("Sprites/Map/ShrubFabs/Shrub5");
	public static GameObject dead_bush = Resources.Load<GameObject> ("Sprites/Map/ShrubFabs/Shrub3");
	public static GameObject bush1 = Resources.Load<GameObject> ("Sprites/Map/ShrubFabs/Shrub11");
	public static GameObject bush2 = Resources.Load<GameObject> ("Sprites/Map/ShrubFabs/Shrub12");
	public static GameObject fern1 = Resources.Load<GameObject> ("Sprites/Map/ShrubFabs/Shrub13");
	public static GameObject cactus = Resources.Load<GameObject> ("Sprites/Map/ShrubFabs/Shrub14");
	public static GameObject rock1 = Resources.Load<GameObject> ("Sprites/Map/RockFabs/Boulder1");
	public static GameObject rock2 = Resources.Load<GameObject> ("Sprites/Map/RockFabs/Boulder2");
}
