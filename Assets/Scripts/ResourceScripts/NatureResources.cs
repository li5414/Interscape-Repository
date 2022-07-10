using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NatureResources : MonoBehaviour
{
	[Header("Tree Prefabs")]
	public GameObject treeBirch;
	public GameObject treeBlue;
	public GameObject treeFig;
	public GameObject treeForest1;
	public GameObject treeForest2;
	public GameObject treeJoshua;
	public GameObject treeOak;
	public GameObject treePalm;
	public GameObject treePine;
	public GameObject treePineSmall;
	public GameObject treeRainforest1;
	public GameObject treeRed;
	public GameObject treeWhite;
	public GameObject treeYellow;

	[Header("Other Plant Prefabs")]
	public GameObject wheat; //7
	public GameObject cane; //9
	public GameObject stick; //5
	public GameObject dead_bush; //3
	public GameObject bush1; //11
	public GameObject bush2; //12
	public GameObject fern1; //13
	public GameObject cactus; //14

	[Header("Rock Prefabs")]
	public GameObject rock1; //1
	public GameObject rock2; //2

	public Dictionary<int, GameObject> idToGameObject;
	// public Dictionary<int, GameObject> gameObjectToId;

	public void Awake() {
		idToGameObject = new Dictionary<int, GameObject>{
			{1, treeBirch},
			{2, treeBlue},
			{3, treeFig},
			{4, treeForest1},
			{5, treeForest2},
			{6, treeJoshua},
			{7, treeOak},
			{8, treePalm},
			{9, treePine},
			{10, treePineSmall},
			{11, treeRainforest1},
			{12, treeRed},
			{13, treeWhite},
			{14, treeYellow},

			{15, wheat},
			{16, cane},
			{17, stick},
			{18, dead_bush},
			{19, bush1},
			{20, bush2},
			{21, fern1},
			{22, cactus},
			{23, rock1},
			{24, rock2},
		};

	// 	gameObjectToId = new Dictionary<GameObject, int>{
	// 		{treeBirch, 1},
	// 		{treeBlue, 2},
	// 		{treeFig, 3},
	// 		{treeForest1, 4},
	// 		{treeForest2, 5},
	// 		{treeJoshua, 6},
	// 		{treeOak, 7},
	// 		{treePalm, 8},
	// 		{treePine, 9},
	// 		{treePineSmall, 10},
	// 		{treeRainforest1, 11},
	// 		{treeRed, 12},
	// 		{treeWhite, 13},
	// 		{treeYellow, 14},

	// 		{wheat, 15},
	// 		{cane, 16},
	// 		{stick, 17},
	// 		{dead_bush, 18},
	// 		{bush1, 19},
	// 		{bush2, 20},
	// 		{fern1, 21},
	// 		{cactus, 22},
	// 		{rock1, 23},
	// 		{rock2, 24},
	// 	};
	}
}
