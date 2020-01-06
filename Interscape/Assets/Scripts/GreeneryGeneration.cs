using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreeneryGeneration : MonoBehaviour {

	int chunkSize = 16;
	BiomeCalculations bCalc;
	ChunkManager cMan;
	float offsetX;
	float offsetY;

	// trees
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
	public GameObject tree_dead1;

	//shrubs
	[Space (10)]
	public GameObject wheat;
	public GameObject cane;
	public GameObject stick;
	public GameObject dead_bush;
	public GameObject bush1;
	public GameObject bush2;
	public GameObject fern1;
	public GameObject cactus;
	public GameObject rock1;
	public GameObject rock2;

	private void Start ()
	{
		// reference other script
		bCalc = GameObject.Find ("System Placeholder").GetComponent<BiomeCalculations> ();
		cMan = GameObject.Find ("System Placeholder").GetComponent<ChunkManager> ();

		System.Random prng = ChunkManager.prng;
		offsetX = prng.Next (-10000, 10000);
		offsetY = prng.Next (-10000, 10000);

	}
	
	public GameObject [,] GeneratePlants (System.Random prng, Vector3Int chunkPos,
		BiomeCalculations.BiomeType [,] biomes, GameObject parent)
	{
		GameObject [,] entities = new GameObject [chunkSize, chunkSize];
		float perlinNoise;
		BiomeCalculations.BiomeType biome;
		GameObject tree1;
		GameObject tree2;
		GameObject tree3;
		GameObject tree4;
		GameObject shrub1; // most common
		GameObject shrub2;
		GameObject shrub3;
		GameObject shrub4; // least common
		bool isTreeBiome;
		float randNum;
		float treeChance = 0;

		// for each tile choose whether to spawn shrub/tree/nothing
		for (int x = 1; x < chunkSize; x++) {
			for (int y = 1; y < chunkSize - 1; y++) {
				isTreeBiome = true;

				biome = biomes [x, y];
				if (biome == BiomeCalculations.BiomeType.Grassland) {
					treeChance = 0.005f;
					tree1 = tree_red;
					tree2 = tree_blue;
					tree3 = tree_yellow;
					tree4 = tree_oak;
					shrub1 = cane;
					shrub2 = wheat;
					shrub3 = dead_bush;
					shrub4 = rock2;
				} else if (biome == BiomeCalculations.BiomeType.Savanna) {
					treeChance = 0.02f;
					tree1 = tree_red;
					tree2 = tree_yellow;
					tree3 = tree_joshua;
					tree4 = tree_dead1;
					shrub1 = dead_bush;
					shrub2 = rock2;
					shrub3 = rock1;
					shrub4 = wheat;
				} else if (biome == BiomeCalculations.BiomeType.Taiga) {
					treeChance = 0.15f;
					tree1 = tree_pine;
					tree2 = tree_birch;
					tree3 = tree_pine_small;
					tree4 = tree_white;
					shrub1 = bush2;
					shrub2 = stick;
					shrub3 = rock2;
					shrub4 = null;
				} else if (biome == BiomeCalculations.BiomeType.SeasonalForest) {
					treeChance = 0.15f;
					tree1 = tree_forest1;
					tree2 = tree_forest2;
					tree3 = tree_forest3;
					tree4 = tree_oak;
					shrub1 = bush1;
					shrub2 = bush2;
					shrub3 = rock2;
					shrub4 = null;
				} else if (biome == BiomeCalculations.BiomeType.Rainforest) {
					treeChance = 0.25f;
					tree1 = tree_rainforest1;
					tree2 = tree_palm;
					tree3 = tree_forest1;
					tree4 = tree_fig;
					shrub1 = cane;
					shrub2 = fern1;
					shrub3 = null;
					shrub4 = null;
				} else if (biome == BiomeCalculations.BiomeType.Tundra) {
					isTreeBiome = false;
					tree1 = null;
					tree2 = null;
					tree3 = null;
					tree4 = null;
					shrub1 = dead_bush;
					shrub2 = bush2;
					shrub3 = rock2;
					shrub4 = rock1;
				} else if (biome == BiomeCalculations.BiomeType.Desert) {
					isTreeBiome = false;
					tree1 = null;
					tree2 = null;
					tree3 = null;
					tree4 = null;
					shrub1 = cactus;
					shrub2 = rock2;
					shrub3 = rock1;
					shrub4 = dead_bush;
				} else {
					isTreeBiome = false;
					tree1 = null;
					tree2 = null;
					tree3 = null;
					tree4 = null;
					shrub1 = null;
					shrub2 = null;
					shrub3 = null;
					shrub4 = null;
				}

				// generate noise values
				perlinNoise = Mathf.PerlinNoise ((chunkPos.x + x + offsetX / 3.5f) * 0.1f,
					(chunkPos.y + y + offsetY / 3.5f) * 0.1f);
				Vector3Int pos = new Vector3Int (chunkPos.x + x, chunkPos.y + y, 198);

				// spawn in trees
				if (isTreeBiome == true) { // check nearby coordinates for trees ???
					treeChance *= perlinNoise; // causes trees to generate in 'clusters'
					randNum = Random.value;

					if (Random.value < treeChance) {
						if (randNum < 0.01f) {
							entities [x, y] = Instantiate (tree4, pos, Quaternion.identity); // rarer tree
						} else if (randNum < 0.34f) {
							entities [x, y] = Instantiate (tree1, pos, Quaternion.identity);
						} else if (randNum > 0.66f) {
							entities [x, y] = Instantiate (tree2, pos, Quaternion.identity);
						} else {
							entities [x, y] = Instantiate (tree3, pos, Quaternion.identity);
						}
						entities [x, y].transform.SetParent (parent.transform, true);
					}
				}

				// spawn in shrubs
				randNum = Random.value;
				if (Random.value < (0.15f * Mathf.InverseLerp (0.5f, 0.9f, perlinNoise)) && entities [x, y] == null) {
					if (randNum < 0.05f && shrub4 != null) {
						entities [x, y] = Instantiate (shrub4, pos, Quaternion.identity); // rarer tree
						entities [x, y].transform.SetParent (parent.transform, true);
					} else if (randNum < 0.3f && shrub3 != null) {
						entities [x, y] = Instantiate (shrub3, pos, Quaternion.identity);
						entities [x, y].transform.SetParent (parent.transform, true);
					} else if (randNum < 0.6f && shrub2 != null) {
						entities [x, y] = Instantiate (shrub2, pos, Quaternion.identity);
						entities [x, y].transform.SetParent (parent.transform, true);
					} else if (shrub1 != null) {
						entities [x, y] = Instantiate (shrub1, pos, Quaternion.identity);
						entities [x, y].transform.SetParent (parent.transform, true);
					}

				}

			}
		}
		return entities;
	}
}