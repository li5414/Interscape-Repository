using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreeneryGeneration : MonoBehaviour {

	int chunkSize = 16;
	public GameObject treeParent;
	public GameObject tree_birch;
	public GameObject tree_blue;
	public GameObject tree_fig;
	public GameObject tree_forest1;
	public GameObject tree_forest2;
	public GameObject tree_forest3;
	public GameObject tree_joshua;
	public GameObject tree_oak;
	public GameObject tree_palm;
	public GameObject tree_pine;
	public GameObject tree_pine_small;
	public GameObject tree_rainforest1;
	public GameObject tree_red;
	public GameObject tree_white;
	public GameObject tree_yellow;
	MyChunkSystem sys;

	private void Start ()
	{
		// reference other script
		sys = GameObject.Find ("System Placeholder").GetComponent<MyChunkSystem> ();

		// tree fabs
		/*tree_birch = Resources.Load<GameObject> ("Assets/Resources/Sprites/Map/TreeFabs/tree_birch");
		tree_blue = Resources.Load<GameObject> ("Assets/Resources/Sprites/Map/TreeFabs/tree_blue");
		tree_joshua = Resources.Load<GameObject> ("Assets/Resources/Sprites/Map/TreeFabs/tree_joshua");
		tree_oak = Resources.Load<GameObject> ("Assets/Resources/Sprites/Map/TreeFabs/tree_oak");
		tree_pine = Resources.Load<GameObject> ("Assets/Resources/Sprites/Map/TreeFabs/tree_pine");
		tree_red = Resources.Load<GameObject> ("Assets/Resources/Sprites/Map/TreeFabs/tree_red");
		tree_white = Resources.Load<GameObject> ("Assets/Resources/Sprites/Map/TreeFabs/tree_white");
		tree_yellow = Resources.Load<GameObject> ("Assets/Resources/Sprites/Map/TreeFabs/tree_yellow");
		*/
	}
	
	public GameObject [,] GeneratePlants (System.Random prng, Vector3Int chunkPos, MyChunkSystem.BiomeType [,] biomes, GameObject parent)
	{
		GameObject [,] entities = new GameObject [chunkSize, chunkSize];
		float perlinNoise;
		MyChunkSystem.BiomeType biome;
		GameObject tree1;
		GameObject tree2;
		GameObject tree3;
		GameObject tree4;
		bool isTrees;
		float randNum;
		float offsetX = prng.Next (-10000, 10000);
		float offsetY = prng.Next (-10000, 10000);
		float treeChance = 0;

		// indicate trees in array (more likely to generate when noise value is low)
		for (int x = 1; x < chunkSize; x++) {
			for (int y = 1; y < chunkSize - 1; y++) {
				isTrees = true;

				biome = biomes [x, y];
				if (biome == MyChunkSystem.BiomeType.Grassland) {
					treeChance = 0.05f;
					tree1 = tree_red;
					tree2 = tree_blue;
					tree3 = tree_yellow;
					tree4 = tree_oak;
				} else if (biome == MyChunkSystem.BiomeType.Savanna) {
					treeChance = 0.05f;
					tree1 = tree_red;
					tree2 = tree_yellow;
					tree3 = tree_joshua;
					tree4 = tree_blue;
				} else if (biome == MyChunkSystem.BiomeType.Taiga) {
					treeChance = 0.2f;
					tree1 = tree_pine;
					tree2 = tree_birch;
					tree3 = tree_pine_small;
					tree4 = tree_white;
				} else if (biome == MyChunkSystem.BiomeType.SeasonalForest) {
					treeChance = 0.25f;
					tree1 = tree_forest1;
					tree2 = tree_forest2;
					tree3 = tree_forest3;
					tree4 = tree_oak;
				} else if (biome == MyChunkSystem.BiomeType.Rainforest) {
					treeChance = 0.35f;
					tree1 = tree_rainforest1;
					tree2 = tree_palm;
					tree3 = tree_forest1;
					tree4 = tree_fig;
				} else {
					isTrees = false;
					tree1 = null;
					tree2 = null;
					tree3 = null;
					tree4 = null;
				}


				// generate noise values
				perlinNoise = Mathf.PerlinNoise (chunkPos.x + x + (offsetX / 3.5f) * 0.1f,
					chunkPos.y + y + (offsetY / 3.5f) * 0.1f);

				// check nearby coordinates for trees ???
				if (isTrees == true) {
					/*if (((entities [x - 1, y] == null && entities [x + 1, y] == null) &&
					(entities [x, y - 1] == null && entities [x, y + 1] == null)) &&
					((entities [x - 1, y - 1] == null && entities [x + 1, y + 1] == null) &&
					(entities [x - 1, y + 1] == null && entities [x + 1, y - 1] == null))) {*/

					treeChance *= perlinNoise - 0.1f;
					// spawning trees in by instantiating prefab
					Vector3Int pos = new Vector3Int (chunkPos.x + x, chunkPos.y + y, 198);
					randNum = Random.value;
					if (Random.value < treeChance) {
						if (randNum < 0.01) {
							entities [x, y] = Instantiate (tree4, pos, Quaternion.identity); // rarer tree
						} else if (randNum < 0.34) {
							entities [x, y] = Instantiate (tree1, pos, Quaternion.identity);
						} else if (randNum > 0.66) {
							entities [x, y] = Instantiate (tree2, pos, Quaternion.identity);
						} else {
							entities [x, y] = Instantiate (tree3, pos, Quaternion.identity);
						}
						entities [x, y].transform.SetParent (parent.transform, true);
					}
				}
			}
		}
		return entities;
	}
}