using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NatureResources
{
	// trees
	public static GameObject tree_birch = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_birch");
	public static GameObject tree_blue = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_blue");
	public static GameObject tree_fig = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_fig");
	public static GameObject tree_forest1 = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_forest1");
	public static GameObject tree_forest2 = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_forest2");
	public static GameObject tree_joshua = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_joshua");
	public static GameObject tree_oak = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_oak");
	public static GameObject tree_palm = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_palm");
	public static GameObject tree_pine = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_pine");
	public static GameObject tree_pine_small = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_pine_small");
	public static GameObject tree_rainforest1 = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_rainforest1");
	public static GameObject tree_red = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_red");
	public static GameObject tree_white = Resources.Load<GameObject>("Sprites/Map/TreeFabs/tree_white");
	public static GameObject tree_yellow = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_yellow");
	public static GameObject tree_dead1 = Resources.Load<GameObject> ("Sprites/Map/TreeFabs/tree_dead1");

	// used only for null checking - can delete later
	public static GameObject[] objects = { tree_birch, tree_blue, tree_fig, tree_forest1, tree_forest2,
  tree_joshua, tree_oak, tree_palm, tree_pine, tree_pine_small, tree_rainforest1, tree_red, tree_white,
  tree_yellow, tree_dead1};

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
