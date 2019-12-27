using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour {

	public GameObject player;
	private Transform playerTrans;
	public const float maxViewDst = 24;
	public Material mapMaterial;

	public static Vector2 viewerPosition;
	static MapGenerator mapGenerator;
	int chunkSize;
	int chunksVisibleInViewDst;

	// chunk dictionary
	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk> ();
	List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk> ();

	void Start ()
	{
		mapGenerator = FindObjectOfType<MapGenerator> ();
		chunkSize = 12;
		chunksVisibleInViewDst = Mathf.RoundToInt (maxViewDst / chunkSize);
		playerTrans = player.transform;
	}

	void Update ()
	{
		// get viewer position
		viewerPosition = new Vector2 (playerTrans.position.x, playerTrans.position.y);
		UpdateVisibleChunks ();
	}

	void UpdateVisibleChunks ()
	{

		for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
			terrainChunksVisibleLastUpdate [i].SetVisible (false);
		}
		terrainChunksVisibleLastUpdate.Clear ();

		// get coordinate of chunk viewer is standing on
		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / chunkSize);


		// go through neighbouring chunks
		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {

				// get position
				Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);


				if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
					// update terrain chunk
					terrainChunkDictionary [viewedChunkCoord].UpdateTerrainChunk ();
					if (terrainChunkDictionary [viewedChunkCoord].IsVisible ()) {
						terrainChunksVisibleLastUpdate.Add (terrainChunkDictionary [viewedChunkCoord]);
					}
				} else {
					// add chunks coordinates to dictionary
					terrainChunkDictionary.Add (viewedChunkCoord, new TerrainChunk (viewedChunkCoord, chunkSize, transform));
				}

			}
		}
	}

	// class to represent chunk object
	public class TerrainChunk {

		GameObject meshObject;
		Vector2 position;
		Bounds bounds;

		// constructor
		public TerrainChunk (Vector2 coord, int size, Transform parent)
		{
			position = coord * size;
			bounds = new Bounds (position, Vector2.one * size); // bounds has to be vithin view dist
			Vector3 positionV3 = new Vector3 (position.x, position.y, 0);

			// create the new object
			meshObject = GameObject.CreatePrimitive (PrimitiveType.Quad);

			// set position, parent and visibility
			meshObject.transform.position = positionV3;
			meshObject.transform.localScale = Vector3.one * size;
			//meshObject.transform.parent = parent;
			SetVisible (false);

			//mapGenerator.RequestMapData (OnMapDataReceived);
		}

		//void OnMapDataReceived (MapData mapData)
		//{
			//mapGenerator.RequestMeshData (mapData, OnMeshDataReceived);
		//}

		//void OnMeshDataReceived (MeshData meshData)
		//{
		//	meshFilter.mesh = meshData.CreateMesh ();
		//}


		public void UpdateTerrainChunk ()
		{
			// determines whether edge of chunk is visible
			float viewerDstFromNearestEdge = Mathf.Sqrt (bounds.SqrDistance (viewerPosition));
			bool visible = viewerDstFromNearestEdge <= maxViewDst;
			SetVisible (visible);
		}

		public void SetVisible (bool visible)
		{
			meshObject.SetActive (visible);
		}

		public bool IsVisible ()
		{
			return meshObject.activeSelf;
		}

	}
}